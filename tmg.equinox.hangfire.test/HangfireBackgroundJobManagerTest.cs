using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire.Configuration;
using tmg.equinox.hangfire;
using tmg.equinox.backgroundjob.Base;
using Moq;

namespace tmg.equinox.hangfire.test
{
    [TestClass]
    public class HangfireBackgroundJobManagerTest
    {
        [TestMethod]
        public void HangfireBackgroundJobManager_Start()
        {

            var mocbackgroundJobConfiguration = GetBackgroundJobConfigurationInstance();

            var moclogprovider = GetLogProvider();

            var mocBackgroundJobServerFactory = GetBackgroundJobServerFactory();

            IConfiguration configuration = new HangfireConfiguration(mocBackgroundJobServerFactory.Object, moclogprovider.Object);

            //Create a mock object of a HangfireBackgroundJobManager class which implements IHangfireBackgroundJobManagerD:\POC\BackGround\tmg.equinox.backgroundjob\Interface\IRunnable.cs
            IBackgroundJobManager mocHangfireBackgroundJobManager = new HangfireBackgroundJobManager(mocbackgroundJobConfiguration.Object, configuration);
            mocHangfireBackgroundJobManager.Start();

            mocBackgroundJobServerFactory.Verify(m => m.Create());

        }
        [TestMethod]
        public void HangfireBackgroundJobManager_Stop()
        {

            var mocbackgroundJobConfiguration = GetBackgroundJobConfigurationInstance();

            var moclogprovider = GetLogProvider();

            var mocBackgroundJobServerFactory = GetBackgroundJobServerFactory();

            IConfiguration configuration = new HangfireConfiguration(mocBackgroundJobServerFactory.Object, moclogprovider.Object);

            //Create a mock object of a HangfireBackgroundJobManager class which implements IHangfireBackgroundJobManager
            IBackgroundJobManager mocHangfireBackgroundJobManager = new HangfireBackgroundJobManager(mocbackgroundJobConfiguration.Object, configuration);
            mocHangfireBackgroundJobManager.WaitToStop();

            mocBackgroundJobServerFactory.Verify(m => m.Dispose());

        }

        private InputValidation StubInputValidation()
        {

            var input = new InputValidation();
            input.InputId = 1;
            input.Name = "Validation";
            input.UserId = "6"; //logged in user ID
            return input;
        }
        [TestMethod]
        public void HangfireBackgroundJobManager_Enqueue()
        {
            var input = StubInputValidation();
            var mocBackgroundJobManager = GetBackgroundJobManager(input);
            var validationService = new ValidationService(mocBackgroundJobManager.Object);                      
            validationService.TestAsyn(input);
            mocBackgroundJobManager.Verify(m => m.EnqueueAsync<TestJob,InputValidation>(input,null));
        } 


        private Moq.Mock<IBackgroundJobManager> GetBackgroundJobManager(InputValidation input)
        {
            var mocBackgroundJobManager = new Moq.Mock<IBackgroundJobManager>();
           // mocBackgroundJobManager.Setup(s => s.Enqueue<TestJob, InputValidation>(input, null)).Callback(() => input.JobId="3");
            mocBackgroundJobManager.Setup(s => s.Enqueue<TestJob, InputValidation>(input, null)).Callback(() => new TestJob().Execute(input));
            mocBackgroundJobManager.Setup(s => s.EnqueueAsync<TestJob, InputValidation>(input, null));
            return mocBackgroundJobManager;
        }


        private Moq.Mock<IBackgroundJobServerFactory> GetBackgroundJobServerFactory()
        {
            //Create a mock object of a backgroundJobServerFactory class which implements IBackgroundJobServerFactory
            var mocBackgroundJobServerFactory = new Moq.Mock<IBackgroundJobServerFactory>();
            mocBackgroundJobServerFactory.Setup(s => s.Create());
            mocBackgroundJobServerFactory.Setup(s => s.Dispose());
            mocBackgroundJobServerFactory.Setup(s => s.IsCreated()).Returns(false);
            return mocBackgroundJobServerFactory;
        }
        private Moq.Mock<ILogProviderFactory> GetLogProvider()
        {
            //Create a mock object of a logprovider class which implements ILogProviderFactory
            var moclogprovider = new Moq.Mock<ILogProviderFactory>();
            moclogprovider.Setup(l => l.CreateLogProvider());
            return moclogprovider;

        }
        private Moq.Mock<IBackgroundJobConfiguration> GetBackgroundJobConfigurationInstance()
        {
            //Create a mock object of a BackgroundJobConfiguration class which implements IBackgroundJobConfiguration
            var mocbackgroundJobConfiguration = new Moq.Mock<IBackgroundJobConfiguration>();
            //mock the properties
            mocbackgroundJobConfiguration.SetupProperty(config => config.IsJobExecutionEnabled, true);
            return mocbackgroundJobConfiguration;
        }
    }



    public class ValidationService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ValidationService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        public void Test(InputValidation input)
        {
            _backgroundJobManager.Enqueue<TestJob, InputValidation>(input,null);

        }
        public void TestAsyn(InputValidation input)
        {
            _backgroundJobManager.EnqueueAsync<TestJob, InputValidation>(input, null);

        }
    }

    public class InputValidation : BaseJobInfo
    {
        public int InputId { get; set; }
    }



    public class TestJob : BackgroundJob<InputValidation>
    { 
        public override void Execute(InputValidation number)
        {
            number.FeatureId= "1";
        }
        //public override void ExecuteQueue(InputValidation number)
        //{
        //   //do insert
        //}
    }

    public class applicationQueue : BackgroundJob<InputValidation>
    {
        public override void Execute(InputValidation number)
        {
            number.FeatureId = "1";
        }

        //public override void ExecuteQueue(InputValidation number)
        //{
        //    //do insert 
        //}
    }
}
