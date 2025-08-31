# HairAI Admin Dashboard

This is the admin dashboard for the HairAI SaaS platform, built with React, TypeScript, and Tailwind CSS.

## Project Structure

- **src/components**: Reusable UI components
- **src/layouts**: Page layouts
- **src/pages**: Page components
- **src/store**: State management with Zustand
- **src/routes**: Routing configuration

## Prerequisites

- Node.js 16+
- npm 7+

## Installation

1. Install dependencies:
   ```bash
   npm install
   ```

2. Create a `.env` file based on `.env.example` and configure the environment variables.

## Usage

Run the development server:
```bash
npm run dev
```

Build for production:
```bash
npm run build
```

Preview the production build:
```bash
npm run preview
```

## Features

- **Dashboard**: Platform metrics and activity overview
- **Clinic Management**: View and manage all clinics
- **Subscription Management**: Monitor subscriptions and plans
- **User Management**: Manage platform users
- **Authentication**: Secure login for super admins

## Tech Stack

- React 18
- TypeScript
- Tailwind CSS
- React Router v6
- Zustand (State Management)
- Recharts (Data Visualization)
- Headless UI (Accessible UI Components)
- Heroicons (Icons)

## Folder Structure

```
src/
├── components/        # Reusable UI components
│   ├── forms/         # Form components
│   └── ui/            # General UI components
├── layouts/           # Page layouts
├── pages/             # Page components
│   └── auth/          # Authentication pages
├── store/             # State management
└── App.tsx            # Main application component
```

## Environment Variables

The dashboard requires the following environment variables:

- `VITE_API_BASE_URL`: The base URL for the backend API
- `VITE_APP_NAME`: The name of the application

## License

This project is licensed under the MIT License.