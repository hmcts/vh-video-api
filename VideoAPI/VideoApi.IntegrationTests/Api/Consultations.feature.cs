// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.0.0.0
//      SpecFlow Generator Version:3.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace VideoApi.IntegrationTests.Api
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Consultations")]
    public partial class ConsultationsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Consultations.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Consultations", "  In order to manage private consultations\r\n  As an API service\r\n  I want to rais" +
                    "e and respond to private consultations", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfully raise a private consultation request")]
        public virtual void SuccessfullyRaiseAPrivateConsultationRequest()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfully raise a private consultation request", null, ((string[])(null)));
#line 6
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 7
    testRunner.Given("I have a conference", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
    testRunner.And("I have a valid raise consultation request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 9
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 10
    testRunner.Then("the response should have the status NoContent and success status True", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Raise private consultation request against non-existent conference")]
        public virtual void RaisePrivateConsultationRequestAgainstNon_ExistentConference()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Raise private consultation request against non-existent conference", null, ((string[])(null)));
#line 12
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 13
    testRunner.Given("I have a conference", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 14
    testRunner.And("I have a nonexistent raise consultation request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 15
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 16
    testRunner.Then("the response should have the status NotFound and success status False", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Raise a private consultation request that fails validation")]
        public virtual void RaiseAPrivateConsultationRequestThatFailsValidation()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Raise a private consultation request that fails validation", null, ((string[])(null)));
#line 18
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 19
    testRunner.Given("I have an invalid raise consultation request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 20
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 21
    testRunner.Then("the response should have the status BadRequest and success status False", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 22
    testRunner.And("the error response message should also contain \'RequestedBy is required\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
    testRunner.And("the error response message should also contain \'RequestedFor is required\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 24
    testRunner.And("the error response message should also contain \'ConferenceId is required\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Raise private consultation request when user requested by does not exist")]
        public virtual void RaisePrivateConsultationRequestWhenUserRequestedByDoesNotExist()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Raise private consultation request when user requested by does not exist", null, ((string[])(null)));
#line 26
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 27
    testRunner.Given("I have a conference", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 28
    testRunner.And("I have a raise consultation request with an invalid requestedBy", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 29
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 30
    testRunner.Then("the response should have the status NotFound and success status False", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Raise private consultation request when user requested for does not exist")]
        public virtual void RaisePrivateConsultationRequestWhenUserRequestedForDoesNotExist()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Raise private consultation request when user requested for does not exist", null, ((string[])(null)));
#line 32
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 33
    testRunner.Given("I have a conference", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 34
    testRunner.And("I have a raise consultation request with an invalid requestedFor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 35
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 36
    testRunner.Then("the response should have the status NotFound and success status False", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfully respond to a private consultation request")]
        public virtual void SuccessfullyRespondToAPrivateConsultationRequest()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfully respond to a private consultation request", null, ((string[])(null)));
#line 41
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 42
    testRunner.Given("I have a conference", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 43
    testRunner.And("I have a valid respond consultation request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 44
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 45
    testRunner.Then("the response should have the status NoContent and success status True", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Respond to private consultation request against non-existent conference")]
        public virtual void RespondToPrivateConsultationRequestAgainstNon_ExistentConference()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Respond to private consultation request against non-existent conference", null, ((string[])(null)));
#line 47
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 48
    testRunner.Given("I have a conference", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 49
    testRunner.And("I have a nonexistent respond consultation request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 50
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 51
    testRunner.Then("the response should have the status NotFound and success status False", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Respond to a private consultation request that fails validation")]
        public virtual void RespondToAPrivateConsultationRequestThatFailsValidation()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Respond to a private consultation request that fails validation", null, ((string[])(null)));
#line 53
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 54
    testRunner.Given("I have an invalid respond consultation request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 55
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 56
    testRunner.Then("the response should have the status BadRequest and success status False", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 57
    testRunner.And("the error response message should also contain \'RequestedBy is required\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 58
    testRunner.And("the error response message should also contain \'RequestedFor is required\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 59
    testRunner.And("the error response message should also contain \'ConferenceId is required\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 60
    testRunner.And("the error response message should also contain \'Answer to request is required\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Respond to private consultation request when user requested by does not exist")]
        public virtual void RespondToPrivateConsultationRequestWhenUserRequestedByDoesNotExist()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Respond to private consultation request when user requested by does not exist", null, ((string[])(null)));
#line 62
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 63
    testRunner.Given("I have a conference", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 64
    testRunner.And("I have a respond consultation request with an invalid requestedBy", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 65
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 66
    testRunner.Then("the response should have the status NotFound and success status False", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Respond to private consultation request when user requested for does not exist")]
        public virtual void RespondToPrivateConsultationRequestWhenUserRequestedForDoesNotExist()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Respond to private consultation request when user requested for does not exist", null, ((string[])(null)));
#line 68
  this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 69
    testRunner.Given("I have a conference", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 70
    testRunner.And("I have a respond consultation request with an invalid requestedFor", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 71
    testRunner.When("I send the request to the endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 72
    testRunner.Then("the response should have the status NotFound and success status False", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
