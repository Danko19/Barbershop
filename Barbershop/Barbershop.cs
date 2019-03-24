using System;
using System.Linq;
using System.Text;

namespace Barbershop
{
    public class Barbershop
    {
        private const int chairsCount = 5;

        public object FindChairLocker = new object();
        public object ServeClientLocker = new object();

        private readonly Chair[] chairs = new Chair[chairsCount];

        public Barbershop()
        {
            for (int i = 0; i < chairsCount; i++)
            {
                chairs[i] = new Chair();
            }
        }

        public Chair FindFreeChair(bool forClient = true)
        {
            return chairs.FirstOrDefault(chair => !chair.Busy && (forClient || chair.Visitor == null));
        }

        public Client FindNextClient()
        {
            return chairs
                .Where(x => x.Busy && x.Visitor is Client client && client.Barber == null)
                .OrderBy(x => x.AttendTime)
                .FirstOrDefault()
                ?.Visitor as Client;
        }

        public bool IsAlreadySeat(IPerson person)
        {
            return chairs.Any(x => x.Visitor == person);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Парикмахерская в {DateTime.Now:T}:");

            foreach (var chair in chairs)
            {
                var visitorName = chair.Visitor?.Name;
                if (visitorName == null)
                    sb.AppendLine($"\t{chair.Name} свободно");
                else sb.AppendLine($"\t{chair.Name} занято. В нем сидит {visitorName}");
            }

            return sb.ToString();
        }
    }
}