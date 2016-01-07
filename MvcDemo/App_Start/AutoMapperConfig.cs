
namespace MvcDemo.App_Start
{
    public class AutoMapperConfig
    {
        public static void Init()
        {
            BLL.DtoMappers.Init();
            DTO.DtoMappers.Init();
        }
    }
}