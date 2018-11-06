using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TransponderReceiver; //Needed in order to use the TransponderReceiver dll

namespace ATM
{
    public class TransponderReceiver : ISubject
    {
        private ITransponderReceiver receiver;

        private List<IObserver> _observers = new List<IObserver>();

        public TransponderReceiver(ITransponderReceiver receiver)
        {
            // This will store the real or the fake transponder data receiver
            this.receiver = receiver;

            // Attach to the event of the real or the fake TDR
            this.receiver.TransponderDataReady += ReceiverOnTransponderDataReady;
        }

        private void ReceiverOnTransponderDataReady(object sender, RawTransponderDataEventArgs e)
        {

            foreach (var data in e.TransponderData)
            {
                List<string> TrackList = data.Split(';').ToList<string>();

                TrackData t = new TrackData(TrackList[0], double.Parse(TrackList[1]), double.Parse(TrackList[2]),
                    double.Parse(TrackList[3]), TrackList[4], 0, 0);

                foreach (IObserver observer in _observers)
                {
                    observer.Update(t);
                }

            }
        }

        public void Attach(IObserver atm)
        {
            _observers.Add(atm);
        }

        public void Detach(IObserver atm)
        {
            _observers.Remove(atm);
        }

        public int getObserverCount()
        {
            return _observers.Count;
        }

    }

}
