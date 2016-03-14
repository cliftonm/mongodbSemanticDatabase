using System;
using System.Data;

using Clifton.Core.ServiceManagement;

namespace WinformExample
{
	public interface ISemanticViewService : IService
	{
		bool HasSelectedRow { get; }
		DataRow SelectedRow { get; }

		int SelectedRowIndex { get; }
		int NumRows { get; }

		DataRow GetRowAt(int idx);
	}

	public interface IAssociatedDataViewService : IService
	{
	}
}
