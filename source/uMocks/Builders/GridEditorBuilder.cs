using System;
using System.Collections.Generic;
using System.Linq;
using Json.Fluently.Builders;
using Json.Fluently.Builders.Abstract;
using Json.Fluently.Syntax;
using Newtonsoft.Json.Linq;
using uMocks.Builders.Abstract;
using uMocks.Syntax;

namespace uMocks.Builders
{
  internal class GridEditorBuilder : IGridEditorBuilder
  {
    public IGridEditorLayoutSyntax CreateNew(string layoutName)
    {
      return new GridEditorSyntax(layoutName);
    }

    private class GridEditorSyntax : IGridEditorLayoutSyntax, IGridEditorContentSyntax
    {
      private readonly ICollection<GridSection> _gridSections = new List<GridSection>();

      private readonly string _layoutName;

      public GridEditorSyntax(string layoutName)
      {
        _layoutName = layoutName;
      }

      public IGridSectionSyntax AddSection(int layoutColumnCount)
      {
        var gridSection = new GridSection(layoutColumnCount);

        _gridSections.Add(gridSection);

        return new GridSectionSyntax(this, gridSection);
      }

      public IGridEditorContentSyntax PutControl(int sectionIndex, int rowIndex, int columnIndex, string alias, JObject value)
      {
        var section = _gridSections.ElementAtOrDefault(sectionIndex);
        if (section == null)
          throw new IndexOutOfRangeException("Given section index is out of range.");

        var row = section.Rows.ElementAtOrDefault(rowIndex);
        if (row == null)
          throw new IndexOutOfRangeException("Given row index is out of range.");

        var column = row.Columns.ElementAtOrDefault(columnIndex);
        if (column == null)
          throw new IndexOutOfRangeException("Given column index is out of range.");

        column.Controls.Add(new GridControl(alias, value));

        return this;
      }

      public IGridEditorContentSyntax PutControl(int sectionIndex, int rowIndex, int columnIndex,
        string alias, Func<IFluentJsonBuilder, IJsonObjectBuilder> objectSyntaxFunc)
      {
        var builder = new FluentJsonBuilder();

        var obj = objectSyntaxFunc(builder).Build();

        return PutControl(sectionIndex, rowIndex, columnIndex, alias, obj);
      }

      public JObject Build()
      {
        var builder = new FluentJsonBuilder();

        var gridObject = builder.CreateNew()
          .WithProperty("name", _layoutName)
          .WithArray("sections", GetGridSections)
          .Build();

        return gridObject;
      }

      private IJsonArrayBuilder GetGridSections(IJsonArraySyntax arraySyntax)
      {
        var builder = new FluentJsonBuilder();

        return arraySyntax.WithItems(_gridSections.Select(gridSection => builder.CreateNew()
          .WithProperty("grid", gridSection.LayoutColumnCount)
          .WithArray("rows", stx => GetGridSectionRows(gridSection, stx))
          .Build()).ToArray());
      }

      private IJsonArrayBuilder GetGridSectionRows(GridSection section, IJsonArraySyntax arraySyntax)
      {
        var builder = new FluentJsonBuilder();

        return arraySyntax.WithItems(section.Rows.Select(gridRow => builder.CreateNew()
          .WithProperty("id", GetComponentId())
          .WithProperty("name", gridRow.LayoutName)
          .WithProperty("hasConfig", false) // TODO: parametrize this value later
          .WithArray("areas", stx => GetGridRowColumns(gridRow, stx))
          .Build()).ToArray());
      }

      private IJsonArrayBuilder GetGridRowColumns(GridRow row, IJsonArraySyntax arraySyntax)
      {
        var builder = new FluentJsonBuilder();

        return arraySyntax.WithItems(row.Columns.Select(gridRowColumn => builder.CreateNew()
          .WithProperty("grid", 12 / row.Columns.Count)
          .WithProperty("allowAll", true) // TODO: parametrize this value later
        //.WithArray("allowed", new JArray()) // TODO: parametrize this value later
          .WithProperty("hasConfig", false) // TODO: parametrize this value later
          .WithArray("controls", stx => GetGridColumnControls(gridRowColumn, stx))
          .Build()).ToArray());
      }

      private IJsonArrayBuilder GetGridColumnControls(GridColumn column, IJsonArraySyntax arraySyntax)
      {
        var builder = new FluentJsonBuilder();

        return arraySyntax.WithItems(column.Controls.Select(control => builder.CreateNew()
          .WithObject("value", stx => stx.CreateNew()
            .WithProperty("id", GetComponentId())
            .WithProperty("dtgeContentTypeAlias", control.Alias)
            .WithObject("value", control.Value))
          .WithObject("editor", stx => stx.CreateNew()
            .WithProperty("alias", control.Alias))
          .WithProperty("active", true)
          .Build()).ToArray());
      }

      private string GetComponentId()
      {
        return Guid.NewGuid().ToString();
      }

      private class GridSectionSyntax : IGridSectionSyntax
      {
        private readonly IGridEditorContentSyntax _gridEditorContentSyntax;

        private readonly GridSection _gridSection;

        public GridSectionSyntax(IGridEditorContentSyntax gridEditorContentSyntax, GridSection section)
        {
          _gridEditorContentSyntax = gridEditorContentSyntax;
          _gridSection = section;
        }

        public IGridRowSyntax AddRow(string rowLayoutName)
        {
          var row = new GridRow(rowLayoutName);

          _gridSection.Rows.Add(row);

          return new GridRowSyntax(this, row);
        }

        public IGridEditorContentSyntax SubmitLayout()
        {
          return _gridEditorContentSyntax;
        }
      }

      private class GridRowSyntax : IGridRowSyntax
      {
        private readonly IGridSectionSyntax _gridSectionSyntax;

        private readonly GridRow _gridRow;

        public GridRowSyntax(IGridSectionSyntax gridSectionSyntax, GridRow row)
        {
          _gridSectionSyntax = gridSectionSyntax;
          _gridRow = row;
        }

        public IGridSectionSyntax WithColumns(int columnCount)
        {
          for (int i = 0; i < columnCount; i++)
            _gridRow.Columns.Add(new GridColumn());

          return _gridSectionSyntax;
        }
      }

      private class GridSection
      {
        public int LayoutColumnCount { get; }

        public ICollection<GridRow> Rows { get; }

        public GridSection(int layoutColumnCount)
        {
          LayoutColumnCount = layoutColumnCount;
          Rows = new List<GridRow>();
        }
      }

      private class GridRow
      {
        public string LayoutName { get;  }

        public ICollection<GridColumn> Columns { get; }

        public GridRow(string layoutName)
        {
          LayoutName = layoutName;
          Columns = new List<GridColumn>();
        }
      }

      private class GridColumn
      {
        public ICollection<GridControl> Controls { get; }

        public GridColumn()
        {
          Controls = new List<GridControl>();
        }
      }

      private class GridControl
      {
        public string Alias { get; }

        public JObject Value { get; }

        public GridControl(string alias, JObject value)
        {
          Alias = alias;
          Value = value;
        }
      }
    }
  }
}
