using System;

namespace Barbershop
{
    public class Chair
    {
        private static int counter = 1;
        private static readonly object locker = new object();

        public Chair()
        {
            lock (locker)
            {
                Name = $"Кресло №{counter++}";
            }
        }

        public string Name { get; }
        public bool Busy { get; private set; }
        public IPerson Visitor { get; private set; }
        public DateTime AttendTime { get; private set; }

        public bool TrySeat(IPerson person)
        {
            if (Busy)
                return false;

            if (person is Client client)
            {
                Seat(client);
                return true;
            }

            if (person is Barber barber)
            {
                Seat(barber);
                return true;
            }

            throw new ArgumentException("Unexpected person");
        }

        private void Seat(Client client)
        {
            Busy = true;
            Visitor = client;
            AttendTime = DateTime.Now;
        }

        private void Seat(Barber barber)
        {
            Busy = false;
            Visitor = barber;
        }

        public void ToLeave()
        {
            Busy = false;
            Visitor = null;
        }
    }
}