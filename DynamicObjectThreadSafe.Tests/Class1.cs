using Xunit;

namespace DynamicObjectThreadSafe.Tests
{

    
    public class ThreadSafeDynamicObjectTests
    {

        [Fact]
        public void It_Can_Accept_Anonymous_Type_initialization()
        {
			dynamic threadSafeToyota = new ThreadSafeDynamicObject(new
			{
				Make = "Toyota",
				Model = "CR-H",
				Propulsion = new
				{
					IsHybrid = true,
					UsesPetrol = true,
					ElectricMotor = true
				}
			});

			Assert.Equal("Toyota", threadSafeToyota.Make);
			Assert.Equal("CR-H", threadSafeToyota.Model);
			Assert.Equal(true, threadSafeToyota.Propulsion.IsHybrid);
			Assert.Equal(true, threadSafeToyota.Propulsion.UsesPetrol);
			Assert.Equal(true, threadSafeToyota.Propulsion.ElectricMotor);
		}

    }
}