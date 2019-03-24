using System.Threading;
using System.Threading.Tasks;

namespace Barbershop
{
    public class Barber : IPerson
    {
        private static int counter = 1;
        private static readonly object locker = new object();

        private readonly Barbershop barbershop;
        public string Name { get; }
        public Chair Chair { get; private set; }

        public Barber(Barbershop barbershop)
        {
            this.barbershop = barbershop;

            lock (locker)
            {
                Name = $"Barber №{counter++}";
            }
        }

        public void Work()
        {
            while (true)
            {
                Task action;
                lock (barbershop.ServeClientLocker)
                {
                    var client = barbershop.FindNextClient();
                    if (client != null)
                    {
                        client.Barber = this;
                        action = Task.Run(() => client.Trim(this));
                    }
                    else action = Task.Run(() => Chill());
                }

                action.Wait();
            }
        }

        private void Chill()
        {
            if (!barbershop.IsAlreadySeat(this))
            {
                lock (barbershop.FindChairLocker)
                {
                    Chair = barbershop.FindFreeChair(false);
                    var tookFreeChair = Chair?.TrySeat(this) ?? false;
                    if (tookFreeChair)
                    {
                        Logger.Write($"{Name} занял свободное кресло и отдыхает");
                    }
                }
            }

            Thread.Sleep(50);
        }
    }
}