using System.Collections.Generic;

public interface IGestureTypeService
{
    List<GestureType> GetAll();
    GestureType GetById(string id);
    void Create(GestureType gestureType);
    void Update(string id, GestureType gestureType);
    void Delete(string id);
}