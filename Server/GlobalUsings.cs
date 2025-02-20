﻿global using System.Reflection;
global using System.Collections.Concurrent;
global using System.Runtime.CompilerServices;
global using System.Text.Json;
global using HogWarpSdk;
global using HogWarpSdk.Systems;
global using HogWarpSdk.Internal;
global using HogWarpSdk.Game.Types;
global using HPlayer = HogWarpSdk.Game.Player;
global using System.Text.RegularExpressions;
global using Serilog.Sinks.SystemConsole.Themes;
global using Serilog;
global using Serilog.Events;
global using Serilog.Settings.Configuration;
global using ILogger = Serilog.ILogger;
global using Pillars.Core.Logging;
global using Pillars.Database.Configuration;
global using MongoDB.Entities;
global using Microsoft.Extensions.DependencyInjection;
global using Pillars.Core.DI.Interfaces;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Pillars.Core.Configuration.Models;
global using Pillars.Core.DI.Helpers;


namespace Pillars;

