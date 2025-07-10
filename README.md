# VagonWEB consist submission tool

This is the repository for a tool that helps with submitting spotted consists of trains to [VagonWEB](https://www.vagonweb.cz), by providing a more user-friendly and mobile-friendly UI. The tool is currently under development and not publicly available yet. However the current source code is available as-is.

## License

The project is licensed under the [GNU AGPLv3](https://choosealicense.com/licenses/agpl-3.0/). This is a slightly modified version of the [GNU GPLv3](https://choosealicense.com/licenses/gpl-3.0/) that explicitly requires modified software providing a service over a network connection (i.e. a backend that is a modified/derived version of the backend in this Git repo) to be open source and licensed under the GNU AGPLv3 as well.

## How to run

> [!NOTE]
> These steps are meant for non-developers wishing to run the app themselves, or for developers that only work with .NET or only work with Javascript/Typescript who wish to run the component they're not familiar with.

The backend is written on top of [.NET 9](https://dotnet.microsoft.com/en-us/), while the frontend is [Vue 3](https://vuejs.org/) with [Vite](https://vite.dev) as build tool. In order to run the app, the following software is required:
 - The [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) for .NET 9 or later, in order to compile and run the backend.
 - [Node.js](https://nodejs.org/en/download) for running the ViteJS build tooling for the frontend. Any version still in support should do.

With the above installed, the backend and frontend can each be started with the terminal commands below. Both will need to run in parallel, so you will need two terminal instances.

 - Backend: `dotnet run --project backend/src/VagonWebSubmissionTool.Web --launch-profile "kestrel"`
 - Frontend: `cd frontend && npm run dev`

Then open [localhost:5173](http://localhost:5173) in order to use the submission tool.