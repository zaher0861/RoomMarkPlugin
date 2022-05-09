using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomMarkPlugin
{
    [Transaction(TransactionMode.Manual)]
    public class AddRoomMark : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            List<Level> levels = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();
            List<ElementId> rooms;
            Transaction transaction = new Transaction(document, "Создание помещений");
            transaction.Start();
            int levelNum = 1;
            foreach (Level level in levels)
            {
                rooms = (List<ElementId>)document.Create.NewRooms2(level);
                foreach (ElementId id in rooms)
                {
                    Room room = document.GetElement(id) as Room;
                    room.Name = $"{levelNum}_{room.Number}";
                }
                levelNum++;
            }
            transaction.Commit();
            return Result.Succeeded;
        }
    }
}
