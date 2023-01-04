using System;

namespace fluXis.Game.Online.Fluxel
{
    public class ResponseListener<T> : IResponseListener
    {
        public Action<FluxelResponse<T>> Callback;

        public ResponseListener(Action<FluxelResponse<T>> callback)
        {
            Callback = callback;
        }

        public void Invoke(string response)
        {
            Callback(FluxelResponse<T>.Parse(response));
        }
    }
}
