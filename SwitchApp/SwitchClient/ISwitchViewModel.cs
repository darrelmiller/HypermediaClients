using System.ComponentModel;
using System.Threading.Tasks;

namespace SwitchClient
{
    public interface ISwitchViewModel : INotifyPropertyChanged
    {
        bool On { get; }
        bool CanTurnOn { get; }
        bool CanTurnOff { get; }
        Task TurnOff();
        Task TurnOn();
    }
}