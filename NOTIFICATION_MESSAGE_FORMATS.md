# WhatsApp vs Email Notification Message Examples

This document shows the difference between Email and WhatsApp notification formats.

## Email Format
Email notifications use HTML formatting with professional styling:

```html
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #d9534f;'>⚠️ Deadline Reminder</h2>
    <p>This is a reminder that the following sale order is <strong>due TODAY</strong>:</p>
    
    <div style='background-color: #f9f9f9; padding: 15px; border-left: 4px solid #d9534f; margin: 10px 0;'>
        <p><strong>Order Number:</strong> ORD-12345</p>
        <p><strong>Client Name:</strong> ABC Company</p>
        <p><strong>Deadline:</strong> Dec 15, 2024</p>
        <p><strong>Status:</strong> In Progress</p>
        <p><strong>Assigned To:</strong> John Doe</p>
        <p><strong>Total Amount:</strong> $5,000.00</p>
    </div>
    
    <p>Please take appropriate action to ensure timely delivery.</p>
    
    <hr style='margin: 20px 0;'/>
    <p style='font-size: 12px; color: #666;'>
        This is an automated notification from the Sale Order Management System.
        <br/>You will continue to receive these notifications until the order status changes to 'Delivered'.
    </p>
</body>
</html>
```

**Characteristics:**
- Professional HTML formatting
- Detailed information with styling
- Tables and structured layout
- Formal tone
- Includes disclaimer and system signature

## WhatsApp Format
WhatsApp notifications use concise text with emojis for visual appeal:

```
🚨 *Deadline Reminder*

*DUE TODAY*

📋 *Order:* ORD-12345
👤 *Client:* ABC Company
📅 *Deadline:* Dec 15, 2024
📊 *Status:* In Progress
✍️ *Assigned:* John Doe
💰 *Amount:* $5,000.00

⚡ Please take action to ensure timely delivery!
```

**Characteristics:**
- Concise and mobile-friendly
- Uses emojis for quick visual scanning
- WhatsApp markdown for bold text (*)
- Brief and to the point
- Action-oriented message
- No formal disclaimer

## Urgency Indicators

### Email
- All urgency levels use the same HTML format
- Text changes: "due TODAY", "due TOMORROW", "due in X days"
- Color scheme remains consistent (#d9534f - red)

### WhatsApp
Different emojis based on urgency:
- 🚨 Critical (due today)
- ⚠️ High (due tomorrow)  
- ⏰ Medium (due in 2 days)
- 📌 Low (due in 3+ days)

## Configuration

Switch between modes in `appsettings.json`:

```json
{
  "Notification": {
    "Mode": "Email"    // or "WhatsApp"
  }
}
```

The system automatically formats messages appropriately based on the selected mode.
