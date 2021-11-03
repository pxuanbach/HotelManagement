using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TagBites.WinSchedulers;
using TagBites.WinSchedulers.Descriptors;

namespace HotelManagement.ViewModels
{
    class SchedulerDataSource : TimeSchedulerDataSource
    {
        private readonly List<ResourceModel> _resources = new List<ResourceModel>();
        private readonly IDictionary<DateTime, IList<TaskModel>> _tasks = new Dictionary<DateTime, IList<TaskModel>>();

        public SchedulerDataSource()
        {
            List<ROOM> list = DataProvider.Instance.DB.ROOMs.Where(x => x.isActive == true).ToList();

            foreach  (ROOM item in list)
            {
                ResourceModel rs = new ResourceModel(item.name, item.id);
                _resources.Add(rs);
            }
        }


        protected override TimeSchedulerResourceDescriptor CreateResourceDescriptor()
        {
            return new TimeSchedulerResourceDescriptor(typeof(ResourceModel));
        }
        protected override TimeSchedulerTaskDescriptor CreateTaskDescriptor()
        {
            return new TimeSchedulerTaskDescriptor(typeof(TaskModel), nameof(TaskModel.Resource), nameof(TaskModel.Interval))
            {
                ColorMember = nameof(TaskModel.Color),
                FontColorMember = nameof(TaskModel.FontColor),
                BorderColorMember = nameof(TaskModel.BorderColor)
            };
        }

        public override IList<object> LoadResources() => _resources.Cast<object>().ToList();
        public override void LoadContent(TimeSchedulerDataSourceView view)
        {
            var resources = view.Resources.Cast<ResourceModel>().ToList();
            var interval = view.Interval;
            var resourcesHashSet = resources.ToDictionary(x => x.ID);

            IList<TaskModel> GetTaskForDate(DateTime date)
            {
                if (!_tasks.ContainsKey(date))
                    _tasks.Add(date.Date, GenerateTasks(date).ToList());

                return _tasks[date];
            };

            for (var i = interval.Start; i < interval.End; i = i.AddDays(1))
            {
                var date = i.Date;
                var tasks = GetTaskForDate(date);
                // Tasks  
                foreach (var task in tasks)
                    if (interval.IntersectsWith(task.Interval) && resources.Contains(task.Resource))
                    {
                        view.AddTask(task);
                    }

                // Interval markers
                if (i.DayOfWeek == DayOfWeek.Sunday)
                    view.AddIntervalMarker(new TimeSchedulerInterval(date, date + TimeSpan.FromDays(1)), m_colors[1]);
            }

            // Now markers
            view.AddMarker(DateTime.Now, Colors.DodgerBlue);
        }

        #region Data generation

        private readonly Random m_random = new Random();
        private readonly Color[] m_colors =
        {
            Color.FromRgb(178, 191, 229),
            Color.FromRgb(178,223, 229),
            Color.FromRgb(178, 229, 203),
            Color.FromRgb(184, 229, 178),
            Color.FromRgb(197, 178, 229),
            Color.FromRgb(216, 229, 178),
            Color.FromRgb(229, 178, 178),
            Color.FromRgb(229,178,197),
            Color.FromRgb(229, 178, 229),
            Color.FromRgb(229, 210, 178),
        };

        public IEnumerable<TaskModel> GenerateTasks(DateTime dateTime)
        {
            Color Lerp(Color color, Color to, float amount)
            {
                return Color.FromRgb(
                    (byte)(color.R + (to.R - color.R) * amount),
                    (byte)(color.G + (to.G - color.G) * amount),
                    (byte)(color.B + (to.B - color.B) * amount));
            }

            var id = 0;
            List<RESERVATION> listReser = DataProvider.Instance.DB.RESERVATIONs
                .Where(x => x.arrival.Value.Day == dateTime.Day
                    && x.arrival.Value.Month == dateTime.Month && x.arrival.Value.Year == dateTime.Year).ToList();

            foreach (RESERVATION item in listReser)
            {
                List<ROOM_BOOKED> listR = DataProvider.Instance.DB.ROOM_BOOKED.Where(x => x.reservation_id == item.id).ToList();
                foreach (ROOM_BOOKED booked in listR)
                {
                    for (var k = 0; k < _resources.Count; k++)
                    {
                        var resource = _resources[k];

                        if (resource.Room_Id == booked.room_id)
                        {
                            var length = item.departure - item.arrival;
                            
                            var color = m_colors[1];
                            var borderColor = Lerp(color, Colors.Black, 0.2f);
                            var fontColor = Color.FromRgb(110, 110, 110);
                            string _status = item.status;
                            string status = "";
                            switch (_status)
                            {
                                case "Operational":
                                    if (item.departure.Value.Day == DateTime.Now.Day && item.departure.Value.Month == DateTime.Now.Month && item.departure.Value.Year == DateTime.Now.Year)
                                    {
                                        status = "Due Out";
                                        color = m_colors[1];
                                    }
                                    else
                                    {
                                        status = "Occupied";
                                        color = m_colors[2];
                                    }
                                    break;
                                case "No Show":
                                    status = "No Show";
                                    color = m_colors[3];
                                    break;
                                case "Completed":
                                case "Cancelled":
                                    status = "Available";
                                    color = m_colors[4];
                                    break;
                                case "Confirmed":
                                case "On Request":
                                    status = "Reserved";
                                    color = m_colors[5];
                                    break;
                                default:
                                    break;
                            }
                            var interval = new TimeSchedulerInterval((DateTime)item.arrival, (TimeSpan)length);
                            var guest = item.main_guest;
                            var sb = new StringBuilder();
                            var resID = item.id.ToString();
                            sb.AppendLine($"Room type: {booked.ROOM.ROOMTYPE.name}");
                            sb.AppendLine($"Reservation ID: {resID}");
                            sb.AppendLine($"Status: {status}");
                            sb.AppendLine($"Main guest: {guest} Name: {item.GUEST.name}");
                            sb.AppendLine($"Date: {interval} ({interval.Duration.TotalDays} Days)");

                            yield return new TaskModel()
                            {
                                Id = ++id,
                                Resource = resource,
                                Interval = interval,
                                Color = color,
                                BorderColor = borderColor,
                                FontColor = fontColor,
                                Text = sb.ToString()
                            };
                        }
                    }
                }
            }
        }

        #endregion

        #region Classes

        public class ResourceModel
        {
            private static int m_id = 0;
            public int ID { get; }
            public string Name { get; }
            public int Room_Id { get; set; }

            public ResourceModel(string name, int id)
            {
                ID = m_id++;
                Name = name;
                Room_Id = id;
            }

            public override string ToString() => Name;
        }
        public class TaskModel
        {
            public int Id { get; set; }
            public ResourceModel Resource { get; set; }
            public TimeSchedulerInterval Interval { get; set; }
            public string Text { get; set; }
            public Color Color { get; set; }
            public Color FontColor { get; set; }
            public Color BorderColor { get; set; }


            public override string ToString() => Text;
        }

        #endregion
    }
}
