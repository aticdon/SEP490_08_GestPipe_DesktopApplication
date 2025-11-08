using GestPipe.Backend.Models;

namespace GestPipe.Backend.Services.IServices
{
    public interface ITrainingGestureService
    {
        //Task AddTrainingGestureAsync(TrainingGesture trainingGesture);
        public void Create(TrainingGesture trainingGesture);
        public TrainingGesture Get(string id);
        public List<TrainingGesture> GetAll();
    }
}
