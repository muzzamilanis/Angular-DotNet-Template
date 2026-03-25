# Full Codebase Change Plan
## Feature: Conversational Order Entry + Live Workflow Diagram

---

## 🗄️ REPO 1 — Backend (.NET Core)

### 1. ➕ ADD `OrderWorkflowStatus.cs` (New Model)
**Path:** `Backend/AspNetCore/SimpleApi/Models/OrderWorkflowStatus.cs`

```csharp
public class OrderWorkflowStatus
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Stage { get; set; }       // "OrderReceived", "AIProcessing", "WriterAssigned", "InProgress", "UnderReview", "Delivered"
    public string AssignedTo { get; set; }  // Writer name
    public string UpdatedBy { get; set; }   // "System" or username
    public DateTime Timestamp { get; set; }
    public string Notes { get; set; }
}
```
**Why:** This is the backbone of the live workflow diagram. Every stage transition gets recorded here.

---

### 2. ✏️ MODIFY `AppDbContext.cs`
**Path:** `Backend/AspNetCore/SimpleApi/Data/AppDbContext.cs`

Add:
```csharp
public DbSet<OrderWorkflowStatus> OrderWorkflowStatuses { get; set; }
```
Then run: `dotnet ef migrations add AddOrderWorkflowStatus && dotnet ef database update`

---

### 3. ➕ ADD `OrderWorkflowHub.cs` (SignalR Hub)
**Path:** `Backend/AspNetCore/SimpleApi/Hubs/OrderWorkflowHub.cs`

```csharp
[Authorize]
public class OrderWorkflowHub : Hub
{
    public async Task JoinOrderGroup(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"order_{orderId}");
    }

    public async Task LeaveOrderGroup(string orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order_{orderId}");
    }
}
```
**Why:** Allows the Angular frontend to subscribe to a specific order's status updates in real time.

---

### 4. ✏️ MODIFY `Program.cs`
**Path:** `Backend/AspNetCore/SimpleApi/Program.cs`

Add hub mapping (it likely already has SignalR registered):
```csharp
app.MapHub<OrderWorkflowHub>("/hubs/orderworkflow");
```

---

### 5. ➕ ADD `OrderWorkflowController.cs`
**Path:** `Backend/AspNetCore/SimpleApi/Controllers/OrderWorkflowController.cs`

Endpoints needed:
```
GET  /api/orderworkflow/{orderId}         → Returns full status history for diagram
POST /api/orderworkflow/update-status     → Called by Python agent at each stage
POST /api/orderworkflow/parse-order       → Accepts natural language, returns structured order (calls Claude/GPT)
```

The `parse-order` endpoint is the AI chatbot brain:
```csharp
[HttpPost("parse-order")]
public async Task<IActionResult> ParseOrder([FromBody] ParseOrderRequest req)
{
    // Call OpenAI/Claude API with system prompt
    // Return structured OrderDto back to Angular
}
```

---

### 6. ✏️ MODIFY `SaleOrdersController.cs` (or equivalent OrderLog controller)
**Path:** `Backend/AspNetCore/SimpleApi/Controllers/SaleOrdersController.cs`

In the `POST` (create order) method, **after** the webhook fires, add:
```csharp
// Insert initial workflow status
await _context.OrderWorkflowStatuses.AddAsync(new OrderWorkflowStatus {
    OrderId = newOrder.Id,
    Stage = "OrderReceived",
    UpdatedBy = currentUser,
    Timestamp = DateTime.UtcNow
});
await _context.SaveChangesAsync();

// Notify SignalR group
await _hubContext.Clients.Group($"order_{newOrder.Id}")
    .SendAsync("WorkflowUpdated", new { stage = "OrderReceived", timestamp = DateTime.UtcNow });
```

---

## 🌐 REPO 1 — Frontend (Angular)

### 7. ➕ ADD `order-chatbot` Component
**Path:** `FrontEnd/src/app/components/order-chatbot/`

Files to create:
- `order-chatbot.component.ts`
- `order-chatbot.component.html`
- `order-chatbot.component.css`

**Behavior:**
- Chat UI with message bubbles
- CSR types natural language order description
- Calls `POST /api/orderworkflow/parse-order`
- AI response shown as structured confirmation card
- "Confirm" button calls existing order creation endpoint
- On confirm → navigates to workflow diagram view for that order

---

### 8. ➕ ADD `order-workflow-diagram` Component
**Path:** `FrontEnd/src/app/components/order-workflow-diagram/`

Files to create:
- `order-workflow-diagram.component.ts`
- `order-workflow-diagram.component.html`
- `order-workflow-diagram.component.css`

**Visual nodes to render:**
```
[Order Received] → [AI Processing] → [Writer Assigned] → [In Progress] → [Under Review] → [Delivered]
```

Each node:
- Green = completed
- Blue pulsing = current stage
- Grey = pending
- Red = delayed (timestamp > deadline)
- Clickable → shows timestamp + actor

---

### 9. ➕ ADD `order-workflow.service.ts`
**Path:** `FrontEnd/src/app/services/order-workflow.service.ts`

```typescript
// Two responsibilities:
// 1. HTTP calls to OrderWorkflowController
// 2. SignalR connection management

export class OrderWorkflowService {
  private hubConnection: HubConnection;

  startConnection(orderId: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('/hubs/orderworkflow', { accessTokenFactory: () => this.authService.getToken() })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start();
    this.hubConnection.invoke('JoinOrderGroup', orderId);
  }

  onWorkflowUpdated(callback: (data: any) => void) {
    this.hubConnection.on('WorkflowUpdated', callback);
  }

  getOrderWorkflow(orderId: number): Observable<WorkflowStatus[]> {
    return this.http.get<WorkflowStatus[]>(`/api/orderworkflow/${orderId}`);
  }
}
```

---

### 10. ✏️ MODIFY Routing (`app.routes.ts`)
**Path:** `FrontEnd/src/app/app.routes.ts`

Add:
```typescript
{ path: 'order-entry', component: OrderChatbotComponent, canActivate: [AuthGuard] },
{ path: 'order-workflow/:id', component: OrderWorkflowDiagramComponent, canActivate: [AuthGuard] },
```

---

### 11. ✏️ MODIFY Sidebar Navigation
**Path:** `FrontEnd/src/app/components/main-layout/`

Add menu item:
```html
<li class="nav-item">
  <a routerLink="/order-entry" class="nav-link">
    <i class="nav-icon fas fa-robot"></i>
    <p>New Order (AI)</p>
  </a>
</li>
```

---

## 🐍 REPO 2 — Python CSR Agent

### 12. ✏️ MODIFY `webhook_server.py`

After successful order processing, push status back to .NET:
```python
def push_status_to_dotnet(order_id: str, stage: str, notes: str = ""):
    headers = {"Authorization": f"Bearer {get_api_token()}"}
    payload = {
        "orderId": order_id,
        "stage": stage,
        "updatedBy": "CSR-Agent",
        "notes": notes
    }
    requests.post(f"{API_BASE_URL}/orderworkflow/update-status", json=payload, headers=headers)
```

Call this at each key point:
```python
push_status_to_dotnet(order_id, "AIProcessing", "Agent analyzing available writers")
# ... after assignment ...
push_status_to_dotnet(order_id, "WriterAssigned", f"Assigned to {writer_name}")
```

---

### 13. ✏️ MODIFY `writer_assignment_agent.py`

Replace file-based checkpointing (`webhook_checkpoint.json`) with PostgreSQL checkpointer:

```python
from langgraph.checkpoint.postgres import PostgresSaver

# Replace:
# memory = MemorySaver()

# With:
conn_string = os.getenv("DATABASE_URL")
checkpointer = PostgresSaver.from_conn_string(conn_string)
graph = workflow.compile(checkpointer=checkpointer)
```

**Why:** Your current JSON file checkpointing means if the agent crashes mid-assignment, the state is lost or corrupted. PostgreSQL checkpointing is durable and atomic.

---

### 14. ➕ ADD status stages to `writer_assignment_agent.py` LangGraph nodes

Add a new node to the graph for status reporting:

```python
def report_status_node(state: AgentState) -> AgentState:
    """Push current stage to .NET API after each major node"""
    push_status_to_dotnet(state['order_id'], state['current_stage'], state.get('notes', ''))
    return state
```

Wire it between each existing node in the graph so every transition is tracked.

---

## 🔐 Security Notes

- The `POST /api/orderworkflow/update-status` endpoint called by Python agent should use a **service account JWT** or a dedicated API key — not user credentials
- Add `[Authorize(Roles = "System,SuperAdmin")]` to that endpoint
- The Python agent's `API_USERNAME/PASSWORD` in `.env` should be a dedicated service account user in your DB with limited permissions

---

## 📊 Summary Table

| # | Action | File | Priority |
|---|--------|------|----------|
| 1 | ➕ Add | `Models/OrderWorkflowStatus.cs` | 🔴 High |
| 2 | ✏️ Modify | `Data/AppDbContext.cs` | 🔴 High |
| 3 | ➕ Add | `Hubs/OrderWorkflowHub.cs` | 🔴 High |
| 4 | ✏️ Modify | `Program.cs` | 🔴 High |
| 5 | ➕ Add | `Controllers/OrderWorkflowController.cs` | 🔴 High |
| 6 | ✏️ Modify | `Controllers/SaleOrdersController.cs` | 🔴 High |
| 7 | ➕ Add | `components/order-chatbot/` | 🔴 High |
| 8 | ➕ Add | `components/order-workflow-diagram/` | 🔴 High |
| 9 | ➕ Add | `services/order-workflow.service.ts` | 🔴 High |
| 10 | ✏️ Modify | `app.routes.ts` | 🟠 Medium |
| 11 | ✏️ Modify | `main-layout` sidebar | 🟠 Medium |
| 12 | ✏️ Modify | `webhook_server.py` | 🔴 High |
| 13 | ✏️ Modify | `writer_assignment_agent.py` (checkpointer) | 🟠 Medium |
| 14 | ✏️ Modify | `writer_assignment_agent.py` (status nodes) | 🔴 High |

**Total: 6 new files, 8 file modifications across both repos.**
