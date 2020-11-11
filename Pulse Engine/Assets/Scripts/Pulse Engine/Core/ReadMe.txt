

Legend:
>/ : region
/ : path



Notes: >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

1- Datas ####################################################################################################################

	- implement in PulseEngine >/data >/ Module
	- add enum Datatype in PulseEngine >/Enum >/ Global
	- Implement interface IData
	- if it must be localisable, inherit from LocalisableData
	- privates serialisables fields
	- avoid at most as possible manages types
	- public properties for field
	- update GetTypefromdata in PulseEditor


2- Libraries ##############################################################################################################

	- create in /Module/Datas
	- inherit from CoreLibrary
	- private serialisable list of the datas it will store
	- override property List<Idata> DataList, by converting it from list<Idata> to list<datas it will store>
	- (optionnal) add a casted property for mainAssetFilter
	- (optionnal) add a casted property for secAssetFilter


3- Editors ##############################################################################################################

	- Derive from PulseEditorMgr
	- Add enum with editor class name in PulseEditor >/ enums >/ ModuleEditors
	- 2) in >/Statics Accesors
		- static bool RegisteredToRefresh --> to avoid the editor to register multiple times to OnRefresh event.
		- static void Refresh(object Dico, DataType _dtype) 
			--> Dico as Dictionary<DataLocation,Idata> : the static dictionnary, where the missing Idatas must be referenced
			--> _dtype : to specify if the OnRefresh was launched for this Editor(_dtype != DataType.XXXXXX), otherwise return
	- in >/Fonctionnal Attributes;
		- const string AssetPath = "" : the child path where to store the scriptables objects (Libraries)
	- 4) in DoorMethods
		- statics OpenEditor(), OpenSelector(Action<object, EditorEventArg>, params objects), OpenModifier(DataLocation, params)
		- for all these above:
			- check if RegisteredToRefresh, register Refresh() to OnRefresh event and active RegisteredToRefresh
	- 5) Recommended usage:
		- Select(Data to store) : to call when selection is made.
		- override OnInitialisation() : link selectAction to Select()
		- override OnHeaderChange() : for actions that must occur on filter asset change.
		- override OnBodyRedraw() : for data details
		- override OnHeaderRedraw() : for header redraw
		- override OnQuit() : for quit actions
		- override OnListChange() : for list change actions
		- override OnFootRedraw() : for page foot redraw
		- override OnListButton() : to implement custom buttons for list view.


		
4- Managers ####################################################################################################################

	- Must have the same name as the namespace
	- add enum ModulesManagers in PulseEngine >/Enum >/ Global of the same name as namespace
	- must have public static void OnDomainReload() from [RuntimeInitializeOnLoadMethod] to reinit on domain reload.

		
5- Components ####################################################################################################################

	- Must be created in the PulseEngine/Modules/Components namespace
        