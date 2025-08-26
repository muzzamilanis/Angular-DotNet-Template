# SmartBizApp Mobile

React Native mobile application for SmartBizApp business management system.

## Overview

This mobile app provides access to the SmartBizApp business management features on mobile devices. It connects to the same ASP.NET Core backend API used by the Angular web application.

## Technology Stack

- **Framework**: React Native with Expo
- **Language**: TypeScript
- **Navigation**: React Navigation v6
- **State Management**: React Context API
- **HTTP Client**: Axios
- **Storage**: AsyncStorage
- **Icons**: Expo Vector Icons

## Features

### Implemented
- ✅ User Authentication (Login/Logout)
- ✅ Dashboard with business metrics
- ✅ Companies listing with real API integration
- ✅ Profile management
- ✅ Responsive mobile UI design
- ✅ Authentication context and token management
- ✅ API service layer with interceptors

### Planned (Coming Soon)
- 🔄 User Registration
- 🔄 User Management (for Super Admin)
- 🔄 Full CRUD operations for Companies
- 🔄 Project Management
- 🔄 Sale Orders management
- 🔄 Push notifications
- 🔄 Offline support
- 🔄 Dark mode theme

## Getting Started

### Prerequisites
- Node.js (v16 or higher)
- npm or yarn
- Expo CLI (`npm install -g @expo/cli`)

### Installation

1. Navigate to the Mobile directory:
   ```bash
   cd Mobile/SmartBizApp
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

4. Use Expo Go app on your mobile device to scan the QR code, or run on simulator:
   ```bash
   npm run ios     # iOS simulator
   npm run android # Android emulator
   npm run web     # Web browser
   ```

## Project Structure

```
Mobile/SmartBizApp/
├── src/
│   ├── components/          # Reusable UI components
│   ├── navigation/          # Navigation configuration
│   │   └── AppNavigator.tsx # Main navigation setup
│   ├── screens/            # Screen components
│   │   ├── LoginScreen.tsx
│   │   ├── DashboardScreen.tsx
│   │   ├── CompaniesScreen.tsx
│   │   └── ...
│   ├── services/           # API services
│   │   └── ApiService.ts   # Main API service
│   ├── types/              # TypeScript type definitions
│   │   └── index.ts        # Shared types
│   └── utils/              # Utility functions and contexts
│       └── AuthContext.tsx # Authentication context
├── App.tsx                 # Main app component
└── package.json
```

## API Configuration

The mobile app connects to the same backend API as the web application. Update the API base URL in `src/services/ApiService.ts`:

```typescript
const API_BASE_URL = 'https://your-api-domain.com/api';
```

For local development, make sure your backend is running and accessible from your mobile device.

## Authentication

The app uses JWT token-based authentication with:
- Access token storage in AsyncStorage
- Automatic token refresh
- Auth context for state management
- Route protection based on authentication status

## Screens

### Dashboard
- Welcome message with user info
- Business metrics overview (companies, projects, orders)
- Quick action cards for navigation
- Account information display

### Companies
- List of all companies with real API data
- Company details including contact information
- Status indicators (Active/Inactive)
- Pull-to-refresh functionality

### Profile
- User account information
- Settings and help actions
- Logout functionality

## Development Notes

### API Integration
- All API calls go through the central `ApiService`
- Automatic token handling with Axios interceptors
- Error handling and user feedback
- Compatible with the existing ASP.NET Core backend

### State Management
- Authentication state managed through React Context
- Local component state for UI-specific data
- AsyncStorage for persistent data (tokens, user info)

### Navigation
- Stack navigator for auth flow
- Bottom tab navigator for main app
- Type-safe navigation with TypeScript

## Building for Production

### Android
```bash
expo build:android
```

### iOS
```bash
expo build:ios
```

## Backend Compatibility

This mobile app is designed to work with the existing SmartBizApp backend API endpoints:

- `/api/auth/login` - User authentication
- `/api/companies` - Company management
- `/api/users` - User management
- `/api/projects` - Project management
- `/api/saleorders` - Order management

Make sure your backend has CORS configured to allow requests from mobile applications.

## Contributing

When adding new features:

1. Follow the existing TypeScript patterns
2. Update types in `src/types/index.ts`
3. Add new API methods to `ApiService.ts`
4. Create reusable components when possible
5. Maintain consistent styling with the existing design

## Troubleshooting

### Common Issues

1. **Metro bundler issues**: Clear cache with `expo start -c`
2. **Network requests failing**: Check API URL and CORS configuration
3. **AsyncStorage issues**: Clear app data or restart development server

### Performance Tips

- Use FlatList for large data sets
- Implement proper loading states
- Optimize images and assets
- Use React.memo for expensive components