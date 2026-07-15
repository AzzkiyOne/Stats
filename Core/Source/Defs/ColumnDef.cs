using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using RimWorld;
using Stats.Columns;
using Stats.Utils.Widgets;
using Verse;

namespace Stats;

public class ColumnDef : Def
{
    public string? labelKey;
    public string? descriptionKey;
    // TODO: Rename to "label something"
    public ColumnLabelXmlNode? title;
    internal Widget LabelWidget => field ??= title?.ToWidget() ?? new Label(LabelCap);
#pragma warning disable CS8618
    public Type columnClass;
#pragma warning restore CS8618
    public List<string> tags = [];

    internal Column<TRecord> MakeColumnInstance<TRecord>()
    {
        Type columnType = columnClass;

        if (columnType.IsGenericTypeDefinition)
        {
            columnType = columnType.MakeGenericType(typeof(TRecord));
        }

        if (typeof(Column<TRecord>).IsAssignableFrom(columnType))
        {
            return (Column<TRecord>)Activator.CreateInstance(columnType, this);
        }
        else
        {
            throw new InvalidCastException($"Column \"${defName}\" does not implement \"${typeof(Column<TRecord>).Name}\".");
        }
    }

    public override void ResolveReferences()
    {
        base.ResolveReferences();

        if (labelKey?.Length > 0 && string.IsNullOrEmpty(label))
        {
            label = labelKey.Translate();
        }

        if (descriptionKey?.Length > 0 && string.IsNullOrEmpty(description))
        {
            description = descriptionKey.Translate();
        }
    }

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string item in base.ConfigErrors())
        {
            yield return item;
        }

        if (tags.Count == 0)
        {
            yield return "no tags.";
        }
    }
}

public sealed class ColumnLabelXmlNode
{
    private readonly List<Element> _elements = [];

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        foreach (object? node in xmlRoot.ChildNodes)
        {
            if (node is XmlText textNode)
            {
                string text = textNode.InnerText.Trim();
                if (text.Length > 0)
                {
                    TextElement element = new(text);
                    _elements.Add(element);
                }
            }
            else if (node is XmlElement elementNode)
            {
                IconElement element = new(elementNode);
                _elements.Add(element);
            }
        }
    }

    internal Widget ToWidget()
    {
        if (_elements.Count == 1)
        {
            return _elements[0].ToWidget();
        }

        return new HorContainer(_elements.Select(elem => elem.ToWidget()).ToArray(), 2f);
    }

    private abstract class Element
    {
        internal abstract Widget ToWidget();
    }

    private sealed class TextElement : Element
    {
        private readonly string _text;

        public TextElement(string text)
        {
            _text = text;
        }

        internal override Widget ToWidget()
        {
            return new Label(_text);
        }
    }

    private sealed class IconElement : Element
    {
        public IconDef Def;

#pragma warning disable CS8618
        public IconElement(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, nameof(Def), xmlRoot.Name);
        }
#pragma warning restore CS8618

        internal override Widget ToWidget()
        {
            return new InlineIcon(Def.Texture, Def.color, Def.scale);
        }
    }
}
