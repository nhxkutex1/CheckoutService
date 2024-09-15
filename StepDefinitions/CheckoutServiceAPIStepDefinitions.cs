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
        private readonly ScenarioContext _scenarioContext;
        private Order _orderModel;
        private HttpResponseMessage? _response;

        public CheckoutServiceAPIStepDefinitions(Order orderModel, ScenarioContext scenarioContext)
        {
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };
            _orderModel = orderModel;
            _scenarioContext = scenarioContext;
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
            var total = result.total;
            _scenarioContext["Total"] = total;
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

        [Then(@"the total bill should be calculated based on current prices")]
        public void ThenTheTotalBillShouldBeCalculatedBasedOnCurrentPrices()
        {
            var actualTotal = (double)_scenarioContext["Total"];
            Assert.AreEqual(_orderModel.GetTotal(), actualTotal);
        }

        [Then(@"the total bill should be £(.*)")]
        public void ThenTheTotalBillShouldBe(double expectedTotal)
        {
            var actualTotal = (double)_scenarioContext["Total"];
            Assert.AreEqual(expectedTotal, actualTotal);
        }

        [Then(@"the API response status code should be (.*)")]
        public void ThenTheAPIResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            Assert.IsNotNull(_response, "The response should not be null.");
            Assert.AreEqual(expectedStatusCode, (int)_response.StatusCode);
        }
    }
}
