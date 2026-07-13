#if TOOLS
using Godot;

[Tool]
public partial class SceneManager : EditorPlugin
{
	public static SceneManagerScene scene;

	public override void _EnterTree()
	{
		
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
	}
}
#endif
