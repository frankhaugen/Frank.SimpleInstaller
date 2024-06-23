# Frank.SimpleInstaller

A simple cross-platform installer tool for dotnet. It is designed to be used with the simplest of dotnet applications. It is not intended to be a full-featured installer tool.

[![GitHub License](https://img.shields.io/github/license/frankhaugen/Frank.SimpleInstaller)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Frank.SimpleInstaller.svg)](https://www.nuget.org/packages/Frank.SimpleInstaller)
[![NuGet](https://img.shields.io/nuget/dt/Frank.SimpleInstaller.svg)](https://www.nuget.org/packages/Frank.SimpleInstaller)

![GitHub contributors](https://img.shields.io/github/contributors/frankhaugen/Frank.SimpleInstaller)
![GitHub Release Date - Published_At](https://img.shields.io/github/release-date/frankhaugen/Frank.SimpleInstaller)
![GitHub last commit](https://img.shields.io/github/last-commit/frankhaugen/Frank.SimpleInstaller)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/frankhaugen/Frank.SimpleInstaller)
![GitHub pull requests](https://img.shields.io/github/issues-pr/frankhaugen/Frank.SimpleInstaller)
![GitHub issues](https://img.shields.io/github/issues/frankhaugen/Frank.SimpleInstaller)
![GitHub closed issues](https://img.shields.io/github/issues-closed/frankhaugen/Frank.SimpleInstaller)


## Installation

Install the tool using the following command:

```bash
dotnet tool install --global Frank.SimpleInstaller.Cli
```

## Simple Usage

Simple usage means that you don't provide any additional configuration. The tool will use prompt the user for the necessary information.

#### Running the following command to package an application:

```bash
dotnet simple-installer pack
```

#### Running the following command to install an application:

```bash
dotnet simple-installer install
```

#### Running the following command to uninstall an application:

```bash
dotnet simple-installer uninstall
```

## Advanced Usage

Advanced usage means that you provide arguments to the tool to avoid user input (some is still required at the moment).

> TODO: Add documentation for advanced usage.