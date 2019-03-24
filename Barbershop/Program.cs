using System;
using System.Threading;
using System.Threading.Tasks;

namespace Barbershop
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Logger.Dispose();

            var random = new Random();
            var barberShop = new Barbershop();
            Task.Run(() => TrackBarbershop(barberShop));
            CreateBarber(barberShop);
            CreateBarber(barberShop);
            while (true)
            {
                CreateClient(barberShop);
                Thread.Sleep(GetRandomInterval(random));
            }
        }

        private static void TrackBarbershop(Barbershop barbershop)
        {
            while (true)
            {
                Logger.Write(barbershop.ToString());
                Logger.Flush();
                Thread.Sleep(1000);
            }
        }

        private static void CreateBarber(Barbershop barbershop)
        {
            Task.Run(() => new Barber(barbershop).Work());
        }

        private static void CreateClient(Barbershop barbershop)
        {
            Task.Run(() => new Client(barbershop).GetHaircut());
        }

        private static int GetRandomInterval(Random random)
        {
            const int baseInterval = 1_000;
            var rndDouble = random.NextDouble() / 0.5;
            rndDouble *= rndDouble;
            return (int)(baseInterval * rndDouble);
        }
    }
}
