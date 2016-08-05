using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.Windows.Data;
using CARS.CARSService;

namespace CARS.SourceCode
{
	public static class CARSServiceClientFactory
	{
		public static CARSServiceClient CreateCARSServiceClient()
		{
#if DEV
			return CreateServiceClient("http://localhost:53894/CARSService.svc");
#elif DEBUG
			return CreateServiceClient("http://cosapxdev7/CARSService/CARSService.svc");
#else
			return CreateServiceClient("http://cars.advent.com/CARSSERVICE/CARSService.svc");
#endif
		}

		public static CARSServiceClient CreateServiceClient(string serviceAddress)
		{
			var endpointAddr = new EndpointAddress(new Uri(Application.Current.Host.Source, serviceAddress));
			var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
			return new CARSServiceClient(binding, endpointAddr);			
		}
	}
}
