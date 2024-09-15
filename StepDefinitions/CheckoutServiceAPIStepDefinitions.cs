using CheckoutService_TestTask.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;

namespace CheckoutService_TestTask.StepDefinitions
{
    [Binding]
    public class CheckoutServiceAPIStepDefinitions
    {
        private readonly HttpClient _client;
        private HttpResponseMessage? _response;
        private double _total;
        private Order _orderModel;

        
        public CheckoutServiceAPIStepDefinitions(Order orderModel)
        {
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };
            _orderModel = orderModel;
        }

        // This hook runs after every scenario
        [AfterScenario]
        public void TearDown()
        {
            _client?.Dispose();        
        }

        [Given(@"a group of (.*) people order (.*) starters, (.*) mains, and (.*) drinks at (.*)")]
        public void GivenAGroupOfPeopleOrderStartersMainsAndDrinksAt(int amoutOfPeople, int starters, int mains, int drinks, string time)
        {
            if (time == "<time>")
                time = DateTime.Now.ToString("HH:mm");

            _orderModel.Items.Add(new OrderItem(Enums.ItemType.Starter, starters, DateTime.Parse(time)));
            _orderModel.Items.Add(new OrderItem(Enums.ItemType.Main, mains, DateTime.Parse(time)));
            _orderModel.Items.Add(new OrderItem(Enums.ItemType.Drink, drinks, DateTime.Parse(time)));
        }

        [When(@"the bill is requested via the API")]
        public void WhenTheBillIsRequestedViaTheAPI()
        {
            var content = new StringContent(JsonConvert.SerializeObject(_orderModel), Encoding.UTF8, "application/json");
            _response = _client.PostAsync("api/checkout/calculate", content).Result;
            var resultContent = _response.Content.ReadAsStringAsync().Result;
            dynamic result = JsonConvert.DeserializeObject(resultContent);
            _total = result.total;
        }

        [Then(@"the total bill should be calculated based on current prices")]
        public void ThenTheTotalBillShouldBeCalculatedBasedOnCurrentPrices()
        {
            Assert.Equals(_orderModel.GetTotal(), _total);
        }

        [Then(@"the total bill should be £(.*)")]
        public void ThenTheTotalBillShouldBe(double expectedTotal)
        {
            Assert.AreEqual(expectedTotal, _total);
        }

        [When(@"(.*) more people join at (.*) and order (.*) mains and (.*) drinks")]
        public void WhenMorePeopleJoinAtAndOrderMainsAndDrinks(int amoutOfPeople, string time, int mains, int drinks)
        {
            _orderModel.Items.Add(new OrderItem(Enums.ItemType.Main, mains, DateTime.Parse(time)));
            _orderModel.Items.Add(new OrderItem(Enums.ItemType.Drink, drinks, DateTime.Parse(time)));
        }

        [When(@"a member cancels their order by amount of (.*) each item")]
        public void WhenAMemberCancelsTheirOrderNumber(int amount)
        {
            _orderModel.RemoveOrderItemEachByAmount(amount);
        }
    }
}
