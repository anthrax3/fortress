using System;
using System.Collections.Generic;
using System.Threading;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Xunit;

namespace Castle.Windsor.Tests
{
	
	public class GenericResolutionDeadlockTestCase
	{
		public interface IMapMember<TSource, TDestination>
		{
			void Map(TSource source, TDestination destination);
		}

		public interface IMap<TSource, TDestination>
		{
			IEnumerable<IMapMember<TSource, TDestination>> Members { get; }
		}

		public interface IMapper<TSource, TDestination>
		{
			void Map(TSource source, TDestination destination);
		}

		internal class Mapper<TSource, TDestination> : IMapper<TSource, TDestination>
		{
			private readonly IMap<TSource, TDestination> map;

			public Mapper(IMap<TSource, TDestination> map)
			{
				this.map = map;
			}

			#region IMapper<TSource,TDestination> Members

			public void Map(TSource source, TDestination destination)
			{
				foreach (var member in map.Members)
					member.Map(source, destination);
			}

			#endregion
		}

		public class ViewService
		{
			private readonly Func<IMapper<Appointment, AppointmentViewModel>> getAppointmentMapper;
			private readonly Func<IMapper<Block, BlockViewModel>> getBlockMapper;

			private IMapper<Appointment, AppointmentViewModel> appointmentMapper;
			private IMapper<Block, BlockViewModel> blockMapper;

			public ViewService(Func<IMapper<Appointment, AppointmentViewModel>> getAppointmentMapper, Func<IMapper<Block, BlockViewModel>> getBlockMapper)
			{
				this.getAppointmentMapper = getAppointmentMapper;
				this.getBlockMapper = getBlockMapper;
			}

			public IMapper<Appointment, AppointmentViewModel> GetAppointmentMapper()
			{
				appointmentMapper = getAppointmentMapper();
				return appointmentMapper;
			}

			public IMapper<Block, BlockViewModel> GetBlockMapper()
			{
				blockMapper = getBlockMapper();
				return blockMapper;
			}
		}

		public class PatientMapper : IMap<Patient, PatientViewModel>
		{
			private readonly int a;
			private readonly object b;

			public PatientMapper(int a, object b)
			{
				this.a = a;
				this.b = b;
				Members = new IMapMember<Patient, PatientViewModel>[0];
			}

			public IEnumerable<IMapMember<Patient, PatientViewModel>> Members { get; }
		}

		public class AppointmentMapper : IMap<Appointment, AppointmentViewModel>
		{
			private readonly Func<int, object, IMapper<Patient, PatientViewModel>> getPatientMapper;

			public AppointmentMapper(Func<int, object, IMapper<Patient, PatientViewModel>> getPatientMapper)
			{
				this.getPatientMapper = getPatientMapper;
				Members = new IMapMember<Appointment, AppointmentViewModel>[0];
			}

			public IEnumerable<IMapMember<Appointment, AppointmentViewModel>> Members { get; }

			public void Map()
			{
				getPatientMapper(0, null).Map(new Patient(), new PatientViewModel());
			}
		}

		public class BlockMapper : IMap<Block, BlockViewModel>
		{
			private readonly Func<object, IMapper<AppointmentCategory, AppointmentCategoryViewModel>> getAppointmentCategoryMapper;

			public BlockMapper(Func<object, IMapper<AppointmentCategory, AppointmentCategoryViewModel>> getAppointmentCategoryMapper)
			{
				this.getAppointmentCategoryMapper = getAppointmentCategoryMapper;
				Members = new IMapMember<Block, BlockViewModel>[0];
			}

			public IEnumerable<IMapMember<Block, BlockViewModel>> Members { get; }

			public void Map()
			{
				getAppointmentCategoryMapper(null).Map(new AppointmentCategory(), new AppointmentCategoryViewModel());
			}
		}

		public class AppointmentCategoryMapper : IMap<AppointmentCategory, AppointmentCategoryViewModel>
		{
			private readonly IMapper<Appointment, AppointmentViewModel> appointmentMapper;

			public AppointmentCategoryMapper(IMapper<Appointment, AppointmentViewModel> appointmentMapper)
			{
				this.appointmentMapper = appointmentMapper;
				Members = new IMapMember<AppointmentCategory, AppointmentCategoryViewModel>[0];
			}

			public IEnumerable<IMapMember<AppointmentCategory, AppointmentCategoryViewModel>> Members { get; }

			public void Map()
			{
				appointmentMapper.Map(new Appointment(), new AppointmentViewModel());
			}
		}

		public interface IViewModel
		{
		}

		public class AppointmentCategory
		{
		}

		public class AppointmentCategoryViewModel : IViewModel
		{
		}

		public class Patient
		{
		}

		public class PatientViewModel : IViewModel
		{
		}

		public class Block
		{
		}

		public class BlockViewModel : IViewModel
		{
		}

		public class Appointment
		{
		}

		public class AppointmentViewModel : IViewModel
		{
		}

		[Fact]
		[Repeat(200)]
		public void No_deadlock_upon_resolving_complex_generic_types_on_multiple_threads()
		{
			// Setup container
			var container = new WindsorContainer();
			container.AddFacility<TypedFactoryFacility>();
			container.Register(
				Component.For(typeof(IMapper<,>)).ImplementedBy(typeof(Mapper<,>)),
				Component.For<ViewService>(),
				Component.For<IMap<Patient, PatientViewModel>>().ImplementedBy<PatientMapper>(),
				Component.For<IMap<Appointment, AppointmentViewModel>>().ImplementedBy<AppointmentMapper>(),
				Component.For<IMap<Block, BlockViewModel>>().ImplementedBy<BlockMapper>(),
				Component.For<IMap<AppointmentCategory, AppointmentCategoryViewModel>>().ImplementedBy<AppointmentCategoryMapper>()
			);

			var viewService = container.Resolve<ViewService>();

			Exception exceptionOnExecute = null;
			// Run resolution on 2 threads
			var t1 = new Thread(() =>
			{
				try
				{
					var appointmentMapper = viewService.GetAppointmentMapper();
					appointmentMapper.Map(new Appointment(), new AppointmentViewModel());
				}
				catch (Exception ex)
				{
					exceptionOnExecute = ex;
				}
			});
			var t2 = new Thread(() =>
			{
				try
				{
					var blockMapper = viewService.GetBlockMapper();
					blockMapper.Map(new Block(), new BlockViewModel());
				}
				catch (Exception ex)
				{
					exceptionOnExecute = ex;
				}
			});
			t1.Start();
			t2.Start();

			container.Dispose();

			Assert.Null(exceptionOnExecute);
		}
	}
}