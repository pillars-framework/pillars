// This file is automatically generated

#pragma warning disable CS8618
#pragma warning disable CS0219

using HogWarpSdk;
using HogWarpSdk.Game;
using HogWarpSdk.Systems;
using HogWarpSdk.Internal;

namespace HogWarp.Replicated
{
    [Replicated(Class = "/HogWarpWeather/BP_HogWarpWeather.BP_HogWarpWeather", Hash = 15532929627310947727)]
    public sealed partial class BpHogWarpWeather : Actor
    {
        [ClientRpc(Function = "RequestWeather", Hash = 878023123782860370)]
        public void RequestWeather(Player player)
        {
            ushort length = 0;
            var data = IBuffer.Create();
            try
            {

                IRpc.Get().Call(player.InternalPlayer, Id, 15532929627310947727, 878023123782860370, data);
            }
            finally
            {
                IBuffer.Release(data);
            }
        }
        public partial void SendWeather(Player player, string weather);

        [ServerRpc(Function = "SendWeather", Hash = 17588033661411641481)]
        public void SendWeather_Impl(Player player, IBuffer data)
        {
            ushort length = 0;
            var weather = data.ReadString();

            SendWeather(player, weather);
        }

        public partial void SendSeason(Player player, int season);

        [ServerRpc(Function = "SendSeason", Hash = 10214483623773692170)]
        public void SendSeason_Impl(Player player, IBuffer data)
        {
            ushort length = 0;
            var season = data.ReadInt32();

            SendSeason(player, season);
        }

        public partial void SendTime(Player player, int hour, int minute, int second);

        [ServerRpc(Function = "SendTime", Hash = 11546380348181940638)]
        public void SendTime_Impl(Player player, IBuffer data)
        {
            ushort length = 0;
            var hour = data.ReadInt32();
            var minute = data.ReadInt32();
            var second = data.ReadInt32();

            SendTime(player, hour, minute, second);
        }

        [ClientRpc(Function = "UpdateSeason", Hash = 14789952481435040489)]
        public void UpdateSeason(Player player, int newSeason)
        {
            ushort length = 0;
            var data = IBuffer.Create();
            try
            {
                data.WriteInt32(newSeason);

                IRpc.Get().Call(player.InternalPlayer, Id, 15532929627310947727, 14789952481435040489, data);
            }
            finally
            {
                IBuffer.Release(data);
            }
        }
        [ClientRpc(Function = "UpdateTime", Hash = 14849233365027045749)]
        public void UpdateTime(Player player, int hours, int minutes, int seconds)
        {
            ushort length = 0;
            var data = IBuffer.Create();
            try
            {
                data.WriteInt32(hours);
                data.WriteInt32(minutes);
                data.WriteInt32(seconds);

                IRpc.Get().Call(player.InternalPlayer, Id, 15532929627310947727, 14849233365027045749, data);
            }
            finally
            {
                IBuffer.Release(data);
            }
        }
        [ClientRpc(Function = "UpdateWeather", Hash = 3942421668184961192)]
        public void UpdateWeather(Player player, string weather)
        {
            ushort length = 0;
            var data = IBuffer.Create();
            try
            {
                data.WriteString(weather);

                IRpc.Get().Call(player.InternalPlayer, Id, 15532929627310947727, 3942421668184961192, data);
            }
            finally
            {
                IBuffer.Release(data);
            }
        }
    }
}
