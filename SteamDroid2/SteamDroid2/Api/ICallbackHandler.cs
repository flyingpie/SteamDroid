using SteamKit2;

namespace SteamDroid.Api
{
    public interface ICallbackHandler
    {
        void HandleCallback(CallbackMsg msg);
    }
}