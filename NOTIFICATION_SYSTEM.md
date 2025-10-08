# Notification System Documentation

## Overview
The notification system supports both Email and WhatsApp notifications with the following key improvements:

## Key Changes Made

### 1. Fixed Background Service Timing
- **Issue**: `DeadlineNotificationService` was waiting 24 hours before the first execution
- **Solution**: Modified to run immediately on application startup, then every 24 hours
- **Impact**: Notifications now work immediately when the application starts

### 2. Added Manual Trigger Endpoints
Four new API endpoints have been added to the Notifications controller:

#### GET `/api/Notifications/dashboard`
- Returns dashboard notifications for the current user
- Shows orders with approaching deadlines based on user permissions

#### POST `/api/Notifications/send-deadline-notifications`
- **Purpose**: Manually trigger deadline notifications (for testing/immediate execution)
- **Access**: Super admins only
- **Functionality**: 
  - Finds all orders with deadlines within 3 days
  - Sends notifications to super admins and assigned writers
  - Uses the configured notification mode (Email or WhatsApp)
  - Returns count of notifications sent

#### POST `/api/Notifications/test-email`
- **Purpose**: Test email configuration and functionality
- **Access**: Super admins only
- **Parameters**: 
  ```json
  {
    "toEmail": "test@example.com", // Optional, defaults to current user
    "subject": "Custom Subject",   // Optional 
    "body": "Custom HTML Body"     // Optional
  }
  ```

#### POST `/api/Notifications/test-whatsapp`
- **Purpose**: Test WhatsApp configuration and functionality
- **Access**: Super admins only
- **Requirements**: Notification mode must be set to "WhatsApp"
- **Parameters**: 
  ```json
  {
    "toPhoneNumber": "+1234567890", // Required, must include country code
    "message": "Custom message"     // Optional, defaults to test message
  }
  ```

### 3. Enhanced Email Service
- **Added Configuration Validation**: Validates all required email settings before sending
- **Better Error Handling**: Logs specific configuration errors instead of generic failures
- **Detailed Logging**: Provides clear feedback on email send status

### 4. Development Environment Support
- **Added SQLite Support**: For development/testing environments
- **Environment-Specific Database**: Uses SQLite in development, SQL Server in production
- **Easier Testing**: No need for SQL Server setup during development

### 5. WhatsApp Notification Support
- **Multi-Channel Notifications**: Support for both Email and WhatsApp notifications
- **Configuration-Based Selection**: Choose notification mode via `appsettings.json`
- **WhatsApp-Friendly Formatting**: Messages optimized for WhatsApp with emojis and concise text
- **Factory Pattern**: Clean abstraction for switching between notification channels

## Configuration

### Notification Mode
The system uses the following notification configuration from `appsettings.json`:

```json
{
  "Notification": {
    "Mode": "Email"  // Options: "Email" or "WhatsApp"
  }
}
```

### Email Configuration

The system uses the following email configuration from `appsettings.json`:

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "SmartBizApp"
  }
}
```

### WhatsApp Configuration

The system uses Twilio's WhatsApp Business API. Configure the following in `appsettings.json`:

```json
{
  "WhatsApp": {
    "AccountSid": "your-twilio-account-sid",
    "AuthToken": "your-twilio-auth-token",
    "FromNumber": "+1234567890"
  }
}
```

**Setting up Twilio WhatsApp:**
1. Create a Twilio account at https://www.twilio.com/
2. Set up WhatsApp Business API in your Twilio console
3. Get your Account SID and Auth Token from the console
4. Configure your WhatsApp-enabled phone number
5. For recipients, use phone numbers in format: `+1234567890` (the service automatically formats them for WhatsApp)

**Important Notes:**
- Phone numbers should include country code (e.g., +1 for US)
- Recipients must have WhatsApp installed and accept your sandbox request (for testing)
- For production, you need an approved Twilio WhatsApp Business Profile

## How It Works

### Notification Modes

The system supports two notification modes that can be switched via configuration:

#### Email Mode (Default)
- Sends formal HTML emails with complete order details
- Professional formatting with tables and styling
- Uses SMTP for delivery

#### WhatsApp Mode
- Sends concise, WhatsApp-friendly messages
- Uses emojis for visual appeal (🚨 for critical, ⚠️ for high priority, etc.)
- Brief format suitable for mobile viewing
- Example WhatsApp message:
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

### Automatic Notifications
1. **Background Service**: Runs immediately on startup, then every 24 hours
2. **Query Logic**: Finds orders with status "In Progress" and deadlines within 3 days
3. **Recipient Logic**:
   - All active super admins receive notifications
   - Assigned writers receive notifications for their orders
4. **Notification Content**: Formatted according to the selected mode (Email or WhatsApp)
5. **Delivery**: Uses the configured notification channel (SMTP for Email, Twilio API for WhatsApp)

### Manual Testing
- Use `/api/Notifications/test-email` to verify email configuration
- Use `/api/Notifications/test-whatsapp` to verify WhatsApp configuration (requires WhatsApp mode)
- Use `/api/Notifications/send-deadline-notifications` to manually trigger notifications (uses configured mode)
- Use `/api/Notifications/dashboard` to see what notifications would be shown
- Switch between Email and WhatsApp by changing `Notification:Mode` in appsettings.json

## Testing the System

### Email Mode Testing
1. **Configure Email Settings** in `appsettings.json`
2. **Set Notification Mode** to "Email"
3. **Start the application** - Background service runs immediately
4. **Check logs** for notification processing:
   ```
   info: SimpleApi.Services.DeadlineNotificationService[0]
         Starting deadline notification check at [timestamp]
   info: SimpleApi.Services.DeadlineNotificationService[0]  
         Found X orders with approaching deadlines
   ```
5. **Use Swagger UI** to test the manual endpoints (authentication required)
6. **Verify email functionality** with the test-email endpoint

### WhatsApp Mode Testing
1. **Set up Twilio Account** and WhatsApp Business API
2. **Configure WhatsApp Settings** in `appsettings.json`
3. **Set Notification Mode** to "WhatsApp"
4. **Update User Phone Numbers** in the database (must include country code)
5. **For Testing**: Join the Twilio WhatsApp sandbox by sending a code to the sandbox number
6. **Start the application** and check logs for WhatsApp message delivery
7. **Use Swagger UI** to trigger manual notifications
8. **Verify messages** on recipient WhatsApp accounts

## Security Features
- All notification endpoints require authentication
- Manual trigger endpoints restricted to super admins only  
- Email and WhatsApp configuration validation prevents silent failures
- Comprehensive logging for troubleshooting
- Sensitive credentials stored in configuration files (use environment variables in production)

## System Status
✅ **Functional**: The notification system is now fully operational with Email and WhatsApp support
✅ **Tested**: Background service runs and processes notifications
✅ **Configurable**: Both Email (SMTP) and WhatsApp (Twilio) settings work correctly
✅ **Secure**: Proper authentication and authorization controls
✅ **Testable**: Manual endpoints available for verification
✅ **Flexible**: Easy switching between Email and WhatsApp modes via configuration

## Troubleshooting

### Email Issues
- Verify SMTP credentials and host settings
- Check if "Less secure app access" is enabled (for Gmail)
- Use App Passwords for Gmail accounts with 2FA enabled
- Check firewall settings for SMTP port 587

### WhatsApp Issues
- Verify Twilio Account SID and Auth Token
- Ensure WhatsApp Business API is enabled in Twilio
- Check phone number format (must include country code with +)
- For testing, recipients must join the Twilio sandbox
- For production, ensure WhatsApp Business Profile is approved
- Check Twilio console for delivery logs and error messages