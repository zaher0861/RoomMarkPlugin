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
            List<ElementId> rooms = null;

            Transaction transaction = new Transaction(document, "Создание помещений");
            transaction.Start();
            foreach (Level level in levels)
            {
                    rooms = (List<ElementId>)document.Create.NewRooms2(level);
            }
            transaction.Commit();
            FilteredElementCollector filteredRooms = new FilteredElementCollector(document)
                    .OfCategory(BuiltInCategory.OST_Rooms);
            List<ElementId> roomId = filteredRooms.ToElementIds() as List<ElementId>;
            Transaction transaction1 = new Transaction(document, "Создание марок");
            transaction1.Start();
            foreach (Level level in levels)
            {
                int levelNum = 1;
                foreach (ElementId id in roomId)
                {
                    Room room = document.GetElement(id) as Room;
                    room.Name = $"{levelNum}_{room.Number}";
                    //LocationPoint locationPoint = room.Location as LocationPoint;
                    //UV roomTagLocation = new UV(locationPoint.Point.X, locationPoint.Point.Y);
                    UV roomTagLocation = new UV(0, 0);
                    LinkElementId roomLinkId = new LinkElementId(id);
                    RoomTag roomTag = document.Create.NewRoomTag(roomLinkId, roomTagLocation, null);
                    levelNum++;
                }
            }
            transaction1.Commit();
            return Result.Succeeded;
        }
    }
}
