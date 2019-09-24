using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using tmg.equinox.repository;
//using NUnit;
//using NUnit.Framework;

namespace tmg.equinox.tests.unittest.repository
{
    [TestClass]
    public class UnitTest1
    {
        private IUnitOfWorkAsync _unitOfWorkAsync { get; set; }
        private IUnitOfWork _unitOfWork { get; set; }

        public UnitTest1()
        {
            //   _unitOfWork = new UnitOfWork();
            _unitOfWorkAsync = new UnitOfWork();
        }

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    UIElement element = _unitOfWork.Repository<UIElement>().Query().Filter(c => c.UIElementID == 1).Get().SingleOrDefault();
        //    Assert.AreEqual(1, element.UIElementID);
        //}

        [TestMethod]
        public void TestMethodForCheckingNotNUllRepositoryAsyncobject()
        {
                     
            //Act
            var _repositoryAsync = _unitOfWorkAsync.RepositoryAsync<UIElement>();
            //Asert
            Assert.IsNotNull(_repositoryAsync);
        }

      

    }

}


// UIElement element = _unitOfWorkAsync.RepositoryAsync<UIElement>().Query().Filter(c => c.UIElementID == 1).Get().SingleOrDefault();
//// UIElement eleFind = _unitOfWorkAsync.RepositoryAsync<UIElement>().FindAsync(1);
// Assert.AreEqual(1, element.UIElementID);
  //[TestMethod]
  //      public void TestMethod_FindById_using_RepositoryAsyncobject()
  //      {

  //          //Act
  //          var _repositoryAsync = _unitOfWorkAsync.RepositoryAsync<UIElement>();
  //          var uielement=_repositoryAsync.FindById(1);
  //          //Asert
  //          Assert.IsNotNull(_repositoryAsync);
  //      }