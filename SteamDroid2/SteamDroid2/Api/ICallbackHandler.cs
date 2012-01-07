using SteamKit2;

namespace SteamDroid2.Api
{
    public interface ICallbackHandler
    {
        void HandleCallback(CallbackMsg msg);
    }
}