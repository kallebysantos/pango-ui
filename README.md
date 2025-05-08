<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="./src/Pango.UI/wwwroot/logo-white.svg">
    <source media="(prefers-color-scheme: light)" srcset="./src/Pango.UI/wwwroot/logo.svg">
    <img alt="Logo" src="./src/Pango.UI/wwwroot/logo.svg" width="350" style="max-width: 100%;">
  </picture>
</p>

# Pango UI

A collection of UI components for Blazor that you can copy and paste into your apps, built with [Tailwind CSS](https://tailwindcss.com).

- [Introduction](#introduction)
- [Why Pango?](#why-pango)
- [How Pango Works](#how-pango-works)
- [Installing](#installing-pango-cli)
- [Adding Components](#installing-pango-ui-components)
- [Using Components](#using-pango-components)
  - [.NET Integrated](#net-integrated)
- [Theming](#theming)
- [Contributing](#contributing)
- [License](#license)

## Introduction

Pango UI is a [Shadcn/UI](https://ui.shadcn.com) inspired component registry for Blazor. It provides beautifully designed and accessible components that you can copy directly into your project, giving you **complete ownership** and **control over your UI**.

## Why Pango?

### The Problem with Traditional Component Libraries

Most UI libraries for Blazor offer components as pre-built packages that you include as dependencies. While this approach works, it presents several limitations:

- **Limited Customization**: You're often restricted to the styling options provided by the library.
- **Version Lock-in**: Updating the library might break your UI if component APIs change.
- **Black Box Development**: When issues occur, debugging becomes challenging as you can't easily inspect or modify the library's internals.
- **Learning Overhead**: Each library has its own patterns and conventions to learn.

### The Shadcn/ui inspiration

[Shadcn/ui](https://ui.shadcn.com/) has been the [top project](https://risingstars.js.org/2024/en#section-all) on JavaScript Rising Stars for two years running—and for good reason. It offers a unique approach: instead of installing a component library as a dependency, you copy and paste the actual component code into your project. You get complete control over customization, styling, and behavior, with components that feel like part of your project.

Pango takes total advantages of this concept:

- **Download, Don't Depend**: Instead of adding a package reference, you download only the components you need directly into your project.
- **Complete Ownership**: Once downloaded, these components are yours to customize, modify, and extend.
- **No Version Anxiety**: Since you own the component code, you're not affected by breaking changes in future releases.
- **Learn Once, Apply Anywhere**: The skills you develop working with Tailwind CSS and component patterns can be applied across different frameworks.

### Why Tailwind CSS?

Pango is built with [Tailwind CSS](https://tailwindcss.com) v4 at its core, which offers significant advantages over traditional CSS frameworks like Bootstrap:

- **Utility-First**: Compose designs directly in your markup with utility classes.
- **Highly Customizable**: Tailor the design system to your brand through simple configuration.
- **Optimized**: Unlike Bootstrap where you need to import the whole library, Tailwind is compiled to a purely and compact CSS, resulting in better performance.
- **Perfect for Component Ownership**: Tailwind's utility classes make it easy to modify component styles without writing new CSS.

With Pango + Tailwind CSS, you can:

- Use `TailwindMerge.NET` for extending component designs
- Directly modify component internals when needed
- Create consistent UI patterns across your entire application
- Transfer your knowledge to other frameworks that use Tailwind (React, Vue, etc.)

## How Pango Works

Pango consists of two main projects:

### Pango CLI

The [Pango CLI](https://github.com/kallebysantos/pango) is a command-line tool that allows you to:

1. **Download components** from any Pango-compatible registry
2. **Create your own registry** of components
3. **Package and distribute** custom components for your team or the community

The CLI acts as the bridge between registries and your project, handling all the file operations needed to seamlessly integrate components into your codebase.

### Pango Registries

Registries are collections of components that follow the Pango specification. The main registry is [Pango UI](https://github.com/kallebysantos/pango-ui), but the architecture supports:

- **Community Registries**: Specialized component collections created by the community
- **Internal Registries**: Custom component sets maintained by teams for consistent branding
- **Framework-Specific Registries**: Components tailored for specific Blazor frameworks (Server, WebAssembly, MAUI)

## Installing Pango CLI

You can install the Pango CLI either globally or locally as a .NET tool.

```bash
# Installing as global
dotnet tool install --global PangoUI.Tool
# or locally with
dotnet tool install --local --create-manifest-if-needed PangoUI.Tool
```

Verify the installation:

```bash
dotnet pango --version
```

## Installing Pango UI Components

Once you have the CLI installed, you can add Pango UI components to your Blazor project.

### Prerequisites

- .NET 8.0 or later
- A Blazor project with Tailwind CSS configured, see [Tailwind .NET](https://github.com/kallebysantos/tailwind-dotnet)
- [`TailwindMerge.NET`](https://github.com/desmondinho/tailwind-merge-dotnet) package installed

### Setup Your Project

First, navigate to your Blazor project directory:

```bash
cd YourBlazorProject
```

Initialize Pango in your project:

```bash
dotnet pango init
```

This will create a `pango-ui.config.json` configuration file in your project root.

### Adding Components

To add a component is pretty simple, use the `add` command:

```bash
dotnet pango add button
```

This will download the Button component files into your project structure, as well update it to match your project namespace.

## Using Pango Components

Once you've added components to your project, you can use them in your Blazor pages and other components.

### The Base Component Structure

Pango components typically include:

1. A main component file (e.g., `Button.razor`) with the component markup
2. A code-behind file (e.g., `Button.razor.cs`) with the component logic
3. Optional CSS or JS files for component-specific behaviour
4. Some components may also include a `PageScript.js` file to allow SSR support, see **TODO! PageScript**

### Tailwind Merging

Pango UI uses [TailwindMerge .NET](https://github.com/desmondinho/tailwind-merge-dotnet) for class merging. This allows you to easily extend component styles without conflicts.

```razor
<Button class="bg-green-500 hove:bg-green-500/70">
  A custom styled button
</Button>
```

### .NET Integrated

**Render Modes:** Pango UI is designed with cross-render mode compatibility in mind. Most components work across different Blazor rendering modes, including Blazor SSR. The goal is to make all components fully cross-render mode compatible, enabling users to enjoy out-of-the-box interactivity without requiring WebAssembly or Server mode.

**Native By Default:** Some components like `input` and `data-table` extends Blazor's built-in components, making it fully compatible with the .NET environment. It means that you can use Pango inside your `EditForm` or just replace `QuickGrid` by `DataTable` without any breaking change.

<details>
  <summary>Edit form example</summary>

  The Input component showcases how Pango UI integrates with Blazor's built-in form capabilities while providing enhanced styling and features. At same time that its work seamlessly with `EditForm` and validation:

```razor
<EditForm Model="@model" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />

    <div class="space-y-4">
        <div>
            <label class="flex-1 flex flex-col gap-1">
              <span class="text-sm font-medium">Name</span>
              <Input @bind-Value="model.Name" />
            </Label>
            <ValidationMessage For="@(() => model.Name)" />
        </div>

        <div>
            <label class="flex-1 flex flex-col gap-1">
              <span class="text-sm font-medium">Email</span>
              <Input @bind-Value="model.Email" type="email" />
            </Label>
            <ValidationMessage For="@(() => model.Email)" />
        </div>

        <Button type="submit">Submit</Button>
    </div>
</EditForm>
```

</details>

## Theming

Pango UI theme is based on [Shadcn/UI Theme](https://ui.shadcn.com/docs/theming) and [Tailwind CSS v4](https://tailwindcss.com/docs/theme). The theme includes:

- Light and dark mode support
- A comprehensive color system
- Consistent spacing and sizing

In order to setup your own theme, its recommended to use the [themux](https://themux.vercel.app/shadcn-themes) customizer.

## Contributing

I'm welcome contributions to both Pango CLI and Pango UI! Check out our contribution guidelines at [GitHub](https://github.com/kallebysantos/pango/blob/main/CONTRIBUTING.md) to get started or just ping me on [Discord](https://discord.com/users/kallebysantos)

## License

Pango UI components is released under the MIT License.
