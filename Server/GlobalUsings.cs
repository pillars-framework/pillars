﻿global using System.Reflection;
global using System.Collections.Concurrent;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using HogWarpSdk;
global using HogWarpSdk.Systems;
global using HogWarpSdk.Internal;
global using HogWarpSdk.Game.Types;
global using NativePlayer = HogWarpSdk.Game.Player;
global using Serilog.Sinks.SystemConsole.Themes;
global using Serilog;
global using Serilog.Events;
global using Serilog.Settings.Configuration;
global using ILogger = Serilog.ILogger;
global using MongoDB.Bson;
global using MongoDB.Driver;
global using MongoDB.Driver.Core.Events;
global using MongoDB.Entities;
global using Pillars.Core.Logging;
global using Pillars.Core.Database.Configuration;
global using Pillars.Core.Database.Controllers;
global using Pillars.Core.DI.Interfaces;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Pillars.Core.Configuration.Models;
global using Pillars.Core.DI.Helpers;
global using Pillars.Core.Server.Data;
global using Pillars.Core.Server.Controllers;
global using Pillars.Core.Server.Events;
global using Pillars.Core.Server.Helpers;
global using Pillars.Core.Server.Models;
global using Pillars.Core.Player.Models;
global using Pillars.Core.Player.Controllers;
global using Pillars.Core.Player.Events;
global using Pillars.Core.Player.Factories;
global using Pillars.Core.Player.Extensions;
global using Pillars.Core.Accounts.Controllers;
global using Pillars.Core.Accounts.Services;
global using Pillars.Entities;
global using Pillars.Core.Helpers;
global using HogWarpSdk.Game;
global using Pillars.Core.House.Data;
global using Pillars.Core.Actors.Models;
global using HogWarp.Replicated;
global using Pillars.Chat.Events;
global using Pillars.Chat.Models;
global using Pillars.Chat.Attributes;
global using Pillars.Chat.Data;
global using Cronos;
global using Pillars.Schedules.Data;
global using Pillars.Schedules.Helpers;
global using Pillars.Schedules.Models;
global using Pillars.Schedules.Services;
global using MongoDB.Bson.Serialization.Attributes;
