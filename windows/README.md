# Vikunja for Windows (WinUI 3)

A native Windows client for [Vikunja](https://vikunja.io), built on **WinUI 3 /
Windows App SDK** with **.NET 8** and the **MVVM** pattern. It talks to a Vikunja
server over the `/api/v2` REST API.

> **Status: scaffold.** This is a working foundation — sign in and browse/complete
> your open tasks — meant to be grown into a full client. It was authored on
> Linux and has **not been compiled**; open it in Visual Studio on Windows to
> build and run (see below).

## What's implemented

- **Sign in** against any Vikunja server (username / password, optional TOTP,
  "keep me signed in") via `POST /api/v2/login`.
- **Task list** across all your projects (`GET /api/v2/tasks`), sorted by due
  date, with a toggle to show or hide completed tasks.
- **Complete a task** inline — the checkbox writes back with
  `PUT /api/v2/tasks/{id}` and reverts on failure.
- **Secure session** — the JWT is stored in the Windows Credential Locker
  (`PasswordVault`), never in plaintext settings or on disk. The server URL is
  remembered so the login form pre-fills next time.
- Automatic **sign-out on 401** so an expired token bounces the user back to the
  login page.

## Architecture

```
src/VikunjaWinUI/
├── App.xaml(.cs)              Application entry point + DI container
├── MainWindow.xaml(.cs)       Root frame, extended title bar, startup routing
├── Models/                    DTOs (TaskItem, Project, Auth)
├── Services/
│   ├── VikunjaApiClient.cs    Typed HttpClient over /api/v2
│   ├── AuthSession.cs         Current server + token, persistence
│   ├── CredentialStore.cs     PasswordVault (secure secret storage)
│   └── SettingsService.cs     Non-secret preferences (ApplicationData)
├── ViewModels/                LoginViewModel, TaskListViewModel (CommunityToolkit.Mvvm)
├── Views/                     LoginPage, TaskListPage
└── Converters/                BoolToVisibilityConverter
```

- **DI** via `Microsoft.Extensions.DependencyInjection`; views resolve their view
  model through `App.GetService<T>()`.
- **MVVM** via `CommunityToolkit.Mvvm` source generators (`[ObservableProperty]`,
  `[RelayCommand]`).
- **Bindings** use compiled `x:Bind`.

## Prerequisites

- Windows 10 build 17763+ or Windows 11
- Visual Studio 2022 (17.10+) with the **.NET Desktop Development** workload and
  the **Windows App SDK C# Templates** component
- .NET 8 SDK

## Build & run

```powershell
# from this folder
dotnet restore VikunjaWinUI.sln
# or open VikunjaWinUI.sln in Visual Studio, set the platform (x64/ARM64),
# and press F5.
```

Because the project is packaged (MSIX, single-project), run it via the
`VikunjaWinUI (Package)` target in Visual Studio for the credential locker and
protocol activation to work.

## Notes & next steps

- **Assets** under `src/VikunjaWinUI/Assets/` are solid-colour placeholders. Swap
  in real branded logos before shipping.
- The `vikunja-winui://` protocol is declared in `Package.appxmanifest` for a
  future OpenID/OAuth browser sign-in flow (the same deep-link pattern the
  Electron desktop app uses).
- Natural next steps: a project navigation pane (`NavigationView`), task detail
  editing, label/assignee display, and offline caching.
