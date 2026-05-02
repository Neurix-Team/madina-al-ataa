using GivingChampion.API.Controllers.Test.v1;

namespace GivingChampion.API.Tests.Controllers;

public class PingControllerTests
{
    [Fact]
    public void Get_ReturnsPong()
    {
        var controller = new PingController();

        var result = controller.Get();

        Assert.Equal("Pong", result);
    }
}
