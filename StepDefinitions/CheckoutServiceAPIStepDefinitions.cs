using CheckoutService_TestTask.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;

namespace CheckoutService_TestTask.StepDefinitions
{
    [Binding]
    public class CheckoutServiceAPIStepDefinitions
    {
        private Order _orderModel;
        private double _total;
        private readonly HttpClient _client;
        private HttpResponseMessage? _response;

        public CheckoutServiceAPIStepDefinitions(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("CheckoutApiClient"); 
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _orderModel = new Order(new List<OrderItem>());
            _total = 0;
            _response = null;
        }

        [Given(@"a group of (.*) people order (.*) starters, (.*) mains, and (.*) drinks at (.*)")]
        public void GivenAGroupOfPeopleOrderStartersMainsAndDrinksAt(int amountOfPeople, int starters, int mains, int drinks, string time)
        {
            if (time == "<time>")
                time = DateTime.Now.ToString("HH:mm");

            _orderModel.Items.Add(new OrderItem(Enums.ItemType.Starter, starters, DateTime.Parse(time)));
            _orderModel.Items.Add(new OrderItem(Enums.ItemType.Main, mains, DateTime.Parse(time)));
            _orderModel.Items.Add(new OrderItem(Enums.ItemType.Drink, drinks, DateTime.Parse(time)));
        }

        [When(@"the bill is requested via the API")]
        public async Task WhenTheBillIsRequestedViaTheAPI()
        {
            var content = new StringContent(JsonConvert.SerializeObject(_orderModel), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync("api/checkout/calculate", content);
            var resultContent = await _response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(resultContent);
            _total = result.total;
        }

        [When(@"(.*) more people join at (.*) and order (.*) mains and (.*) drinks")]
        public void WhenMorePeopleJoinAtAndOrderMainsAndDrinks(int amountOfPeople, string time, int mains, int drinks)
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
            Assert.AreEqual(_orderModel.GetTotal(), _total);
        }

        [Then(@"the total bill should be £(.*)")]
        public void ThenTheTotalBillShouldBe(double expectedTotal)
        {
            Assert.AreEqual(expectedTotal, _total);
        }

        [Then(@"the API response status code should be (.*)")]
        public void ThenTheAPIResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            Assert.IsNotNull(_response, "The response should not be null.");
            Assert.AreEqual(expectedStatusCode, (int)_response.StatusCode);
        }
    }
}
