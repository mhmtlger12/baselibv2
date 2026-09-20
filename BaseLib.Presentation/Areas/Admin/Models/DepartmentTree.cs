using Baselib.Business.DTOs;
namespace BaseLib.Presentation.Areas.Admin.Models;
public sealed record DepartmentTree(DepartmentDto Item, IReadOnlyList<DepartmentTree> Children)
{
    public static IReadOnlyList<DepartmentTree> Build(IReadOnlyList<DepartmentDto> items)
    {
        var byId = items.DistinctBy(x => x.Id).ToDictionary(x => x.Id);
        var visited = new HashSet<int>();
        DepartmentTree Node(DepartmentDto item)
        {
            visited.Add(item.Id);
            return new(item, byId.Values.Where(x => x.ParentDepartmentId == item.Id && !visited.Contains(x.Id)).Select(Node).ToList());
        }
        var roots = byId.Values.Where(x => x.ParentDepartmentId is null || !byId.ContainsKey(x.ParentDepartmentId.Value)).Select(Node).ToList();
        foreach(var item in byId.Values) if (!visited.Contains(item.Id)) roots.Add(Node(item));
        return roots;
    }
}
