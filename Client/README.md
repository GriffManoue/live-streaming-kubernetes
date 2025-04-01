# KubeStream Client (Angular Frontend)

This is the Angular frontend application for the **Live Streaming Kubernetes Application**. It provides the user interface for interacting with the platform.

For details on the overall project architecture, backend services, and setup instructions for the entire application (including backend and infrastructure), please refer to the [main project README](../README.md).

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 19.2.3.

## Key Features

-   User Registration and Login
-   Browsing active live streams
-   Viewing live streams using HLS (via Shaka Player)
-   User profile and stream settings management (including generating stream keys)
-   (Planned: Following users, chat, etc.)

## Development server

Ensure the backend services are running (see main README). Then, to start the local development server for the client:

```bash
# Navigate to this directory (Client/) if you aren't already here
npm install # If you haven't installed dependencies yet
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files. 

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Karma](https://karma-runner.github.io) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
