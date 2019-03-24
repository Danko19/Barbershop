using System;
using System.Diagnostics;
using System.Threading;

namespace Barbershop
{
    public class Client : IPerson
    {
        private static int counter = 1;
        private static readonly object locker = new object();
        private static readonly Random random = new Random();

        public string Name { get; }
        public bool Trimmed { get; private set; }
        public Barber Barber { get; set; }

        private readonly Barbershop barbershop;
        private readonly TimeSpan waitTime;

        public Client(Barbershop barbershop)
        {
            this.barbershop = barbershop;

            lock (locker)
            {
                Name = $"Client №{counter++}";
                waitTime = TimeSpan.FromSeconds(random.Next(5, 10));
            }
        }

        public void GetHaircut()
        {
            Chair chair;
            bool seatTaken;
            lock (barbershop.FindChairLocker)
            {
                chair = barbershop.FindFreeChair();
                seatTaken = chair != null && chair.TrySeat(this);
            }

            if (!seatTaken)
            {
                Logger.Write($"{Name} не смог подстричься, в парикмахерской не оказалось свободных мест");
                return;
            }

            var sw = new Stopwatch();
            sw.Start();
            Logger.Write($"{Name} ожидает барбера");

            do
            {
                lock (barbershop.ServeClientLocker)
                {
                    if (Barber != null) break;
                    if (sw.Elapsed > waitTime)
                    {
                        chair.ToLeave();
                        Logger.Write($"{Name} не смог дождаться барбера за {waitTime.Seconds} секунд и ушел");
                        return;
                    }
                }
                Thread.Sleep(10);

            } while (true);

            while (!Trimmed)
            {
            }

            chair.ToLeave();
            Logger.Write($"{Name} подстрижен и довольный уходит домой");
        }

        public void Trim(Barber barber)
        {
            lock (barbershop.FindChairLocker)
            {
                if (barber.Chair != null && !barber.Chair.Busy)
                    barber.Chair.ToLeave();
            }
            Logger.Write($"{barber.Name} начал стричь {Name}");
            Thread.Sleep(new Random().Next(2_000, 3_000));
            Trimmed = true;
            Logger.Write($"{barber.Name} закончил стричь {Name}");
        }
    }
}