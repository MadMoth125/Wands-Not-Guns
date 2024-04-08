using Sirenix.OdinInspector;
using System;

// define desired attributes here
[InlineEditor(InlineEditorObjectFieldModes.Foldout, Expanded = true)]
[IncludeMyAttributes]
public class ShowPropertyInlineEditorAttribute : Attribute
{
	
}