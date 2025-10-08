# API Testing Guide for Notification System

This guide provides examples for testing the notification endpoints using Swagger UI or any HTTP client.

## Prerequisites
- Application must be running
- You must be authenticated with a Super Admin account
- JWT token must be included in Authorization header

## Endpoints Overview

### 1. GET /api/Notifications/dashboard
**Purpose**: View pending notifications for the current user

**Authentication**: Required (any authenticated user)

**Example Response**:
```json
{
  "notifications": [
    {
      "saleOrderId": 123,
      "orderNumber": "ORD-12345",
      "clientName": "ABC Company",
      "deadline": "2024-12-15T00:00:00",
      "daysUntilDeadline": 2,
      "assignedTo": "John Doe",
      "totalAmount": 5000.00,
      "status": "In Progress",
      "urgencyLevel": "Medium",
      "message": "Order ORD-12345 deadline approaching in 2 days"
    }
  ],
  "totalCount": 1,
  "hasCriticalNotifications": false
}
```

---

### 2. POST /api/Notifications/send-deadline-notifications
**Purpose**: Manually trigger deadline notifications for all approaching orders

**Authentication**: Required (Super Admin only)

**Request Body**: None (empty object `{}`)

**Example Request**:
```json
{}
```

**Example Response**:
```json
{
  "message": "Deadline notifications sent successfully",
  "ordersFound": 5,
  "notificationsSent": 5
}
```

**Notes**:
- Uses the notification mode configured in appsettings.json (Email or WhatsApp)
- Sends to current user for testing purposes
- Only processes orders with deadlines within 3 days

---

### 3. POST /api/Notifications/test-email
**Purpose**: Test email configuration by sending a test email

**Authentication**: Required (Super Admin only)

**Request Body**:
```json
{
  "toEmail": "test@example.com",        // Optional, defaults to current user email
  "subject": "Test Email Subject",      // Optional, uses default if not provided
  "body": "<h1>Test Message</h1>"      // Optional, uses default if not provided
}
```

**Minimal Request** (uses all defaults):
```json
{}
```

**Example Response**:
```json
{
  "message": "Test email sent successfully",
  "sentTo": "test@example.com",
  "sentAt": "2024-12-15T10:30:00"
}
```

**Error Response** (when email config is invalid):
```json
{
  "message": "Failed to send test email",
  "error": "Email configuration error: SmtpHost is not configured"
}
```

---

### 4. POST /api/Notifications/test-whatsapp
**Purpose**: Test WhatsApp configuration by sending a test message

**Authentication**: Required (Super Admin only)

**Prerequisites**:
- `Notification:Mode` must be set to "WhatsApp" in appsettings.json
- WhatsApp configuration must be valid in appsettings.json

**Request Body**:
```json
{
  "toPhoneNumber": "+1234567890",      // Required, must include country code
  "message": "Custom test message"     // Optional, uses default if not provided
}
```

**Minimal Request**:
```json
{
  "toPhoneNumber": "+1234567890"
}
```

**Example Response**:
```json
{
  "message": "Test WhatsApp message sent successfully",
  "sentTo": "+1234567890",
  "sentAt": "2024-12-15T10:30:00"
}
```

**Error Response** (when mode is not WhatsApp):
```json
{
  "message": "WhatsApp mode is not configured. Please set Notification:Mode to 'WhatsApp' in appsettings.json"
}
```

**Error Response** (when phone number is missing):
```json
{
  "message": "No recipient phone number provided. Please include country code (e.g., +1234567890)"
}
```

---

## Testing Workflows

### Workflow 1: Testing Email Notifications

1. **Configure Email Settings**:
   ```json
   {
     "Email": {
       "SmtpHost": "smtp.gmail.com",
       "SmtpPort": "587",
       "Username": "your-email@gmail.com",
       "Password": "your-app-password",
       "FromEmail": "your-email@gmail.com",
       "FromName": "SmartBizApp"
     },
     "Notification": {
       "Mode": "Email"
     }
   }
   ```

2. **Test Email Configuration**:
   - Endpoint: `POST /api/Notifications/test-email`
   - Body: `{ "toEmail": "your-test-email@example.com" }`

3. **Verify Email Received**:
   - Check inbox for test email
   - Verify sender, subject, and content

4. **Test with Real Orders**:
   - Endpoint: `POST /api/Notifications/send-deadline-notifications`
   - Body: `{}`
   - Check email for deadline notifications

### Workflow 2: Testing WhatsApp Notifications

1. **Set Up Twilio Account**:
   - Create account at https://www.twilio.com/
   - Enable WhatsApp Business API
   - Get Account SID and Auth Token
   - For testing, join the sandbox

2. **Configure WhatsApp Settings**:
   ```json
   {
     "WhatsApp": {
       "AccountSid": "your-twilio-account-sid",
       "AuthToken": "your-twilio-auth-token",
       "FromNumber": "+1234567890"
     },
     "Notification": {
       "Mode": "WhatsApp"
     }
   }
   ```

3. **Test WhatsApp Configuration**:
   - Endpoint: `POST /api/Notifications/test-whatsapp`
   - Body: `{ "toPhoneNumber": "+1234567890" }`

4. **Verify WhatsApp Message Received**:
   - Check WhatsApp on the recipient phone
   - Verify message format and emojis

5. **Test with Real Orders**:
   - Endpoint: `POST /api/Notifications/send-deadline-notifications`
   - Body: `{}`
   - Check WhatsApp for deadline notifications

### Workflow 3: Switching Between Modes

1. **Start with Email Mode**:
   - Set `"Notification": { "Mode": "Email" }`
   - Test: `POST /api/Notifications/send-deadline-notifications`
   - Verify email received

2. **Switch to WhatsApp Mode**:
   - Update config: `"Notification": { "Mode": "WhatsApp" }`
   - Restart application (or wait for config reload)
   - Test: `POST /api/Notifications/send-deadline-notifications`
   - Verify WhatsApp message received

3. **Check Application Logs**:
   - Look for: `"Using notification mode: email"` or `"Using notification mode: whatsapp"`

---

## Common Error Scenarios

### Email Errors

**Error**: "Email configuration error: SmtpHost is not configured"
- **Solution**: Add SmtpHost to Email section in appsettings.json

**Error**: "Authentication failed"
- **Solution**: 
  - For Gmail: Enable "Less secure app access" or use App Password
  - Verify Username and Password are correct

**Error**: "Connection timeout"
- **Solution**: Check firewall settings for port 587

### WhatsApp Errors

**Error**: "WhatsApp mode is not configured"
- **Solution**: Set `Notification:Mode` to "WhatsApp" in appsettings.json

**Error**: "WhatsApp configuration error: AccountSid is not configured"
- **Solution**: Add Twilio Account SID to WhatsApp section

**Error**: "Twilio API error: 21211 - Invalid To phone number"
- **Solution**: Ensure phone number includes country code (e.g., +1234567890)

**Error**: "Twilio API error: 21608 - Unable to create record"
- **Solution**: For testing, recipient must join Twilio sandbox first

---

## Using Swagger UI

1. **Navigate to Swagger**:
   - URL: `http://localhost:5000/swagger` (or your app URL)

2. **Authenticate**:
   - Click "Authorize" button
   - Enter: `Bearer your-jwt-token`
   - Click "Authorize" again

3. **Find Notifications Endpoints**:
   - Expand "Notifications" section
   - Choose endpoint to test

4. **Execute Request**:
   - Click "Try it out"
   - Fill in request body (if needed)
   - Click "Execute"
   - View response below

---

## Using cURL

### Test Email
```bash
curl -X POST "http://localhost:5000/api/Notifications/test-email" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"toEmail": "test@example.com"}'
```

### Test WhatsApp
```bash
curl -X POST "http://localhost:5000/api/Notifications/test-whatsapp" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"toPhoneNumber": "+1234567890"}'
```

### Send Deadline Notifications
```bash
curl -X POST "http://localhost:5000/api/Notifications/send-deadline-notifications" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{}'
```

### Get Dashboard Notifications
```bash
curl -X GET "http://localhost:5000/api/Notifications/dashboard" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## Tips for Testing

1. **Start Simple**: Test with test-email or test-whatsapp first
2. **Check Logs**: Always monitor application logs for detailed error messages
3. **Verify Configuration**: Ensure all required settings are in appsettings.json
4. **Test Both Modes**: Verify both Email and WhatsApp work independently
5. **Use Test Data**: Create test orders with approaching deadlines
6. **Monitor Delivery**: Check both email inbox and WhatsApp for messages
7. **Twilio Console**: Check Twilio logs for WhatsApp delivery status

---

## Security Notes

- All endpoints require authentication
- test-email, test-whatsapp, and send-deadline-notifications require Super Admin role
- JWT tokens expire after configured time (default: 24 hours)
- Never share credentials or tokens
- Use environment variables for sensitive configuration in production
