using System;

public interface IHasProgress
{
    public event EventHandler<Event_OnProgressChangedArgs> Event_OnProgressChanged;
    public class Event_OnProgressChangedArgs : EventArgs
    {
        public float ProgressNormalized;
    }
}
