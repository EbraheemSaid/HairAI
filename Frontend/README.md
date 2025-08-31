# HairAI Frontend

This directory contains the frontend applications for the HairAI SaaS platform.

## Project Structure

- **App**: The main clinical application for doctors and clinic admins
- **AdminDashboard**: The SuperAdmin dashboard for platform management

## Prerequisites

- Node.js 16+
- npm 7+

## Installation

1. Navigate to the desired app directory:
   ```
   cd App  # or cd AdminDashboard
   ```

2. Install dependencies:
   ```
   npm install
   ```

3. Create a `.env` file based on `.env.example` and configure the environment variables.

## Usage

Run the development server:
```
npm run dev
```

Build for production:
```
npm run build
```

Preview the production build:
```
npm run preview
```