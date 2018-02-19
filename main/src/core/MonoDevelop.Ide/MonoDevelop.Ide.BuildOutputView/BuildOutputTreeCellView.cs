using System.Collections.Generic;
using Xwt;
using Xwt.Drawing;

namespace MonoDevelop.Ide.BuildOutputView
{
	class BuildOutputTreeCellView : CanvasCellView
	{
		public double CellWidth { get; set; }

		public Color BackgroundColor { get; set; }
		public Color StrongSelectionColor { get; set; }
		public Color SelectionColor { get; set; }
		public Color CellColor { get; set; }

		public bool UseStrongSelectionColor { get; set; }

		public IDataField<bool> HasBackgroundColorField { get; set; }

		public BuildOutputTreeCellView () 
		{
			BackgroundColor = Colors.Blue; // Styles.CellBackgroundColor;
			StrongSelectionColor = Colors.Magenta; // Styles.CellStrongSelectionColor;
			SelectionColor = Colors.Yellow; // Styles.CellSelectionColor;
			CellColor = Colors.Black;
		}

		int packageDescriptionFontSize = 13;

		const int packageDescriptionPaddingHeight = 5;
		const int packageIdRightHandPaddingWidth = 5;
		const int linesDisplayedCount = 4;

		const int checkBoxAreaWidth = 36;
		const int packageImageAreaWidth = 54;
		const int packageDescriptionLeftOffset = checkBoxAreaWidth + packageImageAreaWidth + 8;

		WidgetSpacing packageDescriptionPadding = new WidgetSpacing (5, 5, 5, 10);
		WidgetSpacing packageImagePadding = new WidgetSpacing (0, 0, 0, 5);
		WidgetSpacing checkBoxPadding = new WidgetSpacing (10, 0, 0, 10);

		FilteredBuildOutputNode BuildOutputNode { get; set; }

		protected override void OnDraw(Context ctx, Xwt.Rectangle cellArea)
		{
			FillCellBackground (ctx);
			UpdateTextColor (ctx);

			// Package description.
			var descriptionTextLayout = new TextLayout ();
			descriptionTextLayout.Font = descriptionTextLayout.Font.WithSize (packageDescriptionFontSize);
			descriptionTextLayout.Width = 100; // - packageDescriptionPadding.HorizontalSpacing - packageDescriptionLeftOffset;
			descriptionTextLayout.Height = 30;// - packageDescriptionPadding.VerticalSpacing;
			descriptionTextLayout.Text = BuildOutputNode.Message;
			//descriptionTextLayout.Trimming = TextTrimming.Word;

			ctx.DrawTextLayout (
				descriptionTextLayout,
				cellArea.Left + packageDescriptionPadding.Left + packageDescriptionLeftOffset,
				cellArea.Top + packageDescriptionPaddingHeight + packageDescriptionPadding.Top);
		}

		protected override Size OnGetRequiredSize ()
		{
			var layout = new TextLayout ();
			layout.Text = "W";
			layout.Font = layout.Font.WithSize (packageDescriptionFontSize);
			Size size = layout.GetSize ();
			return new Size (CellWidth, size.Height * linesDisplayedCount + packageDescriptionPaddingHeight + packageDescriptionPadding.VerticalSpacing);
		}


		protected override void OnDataChanged()
		{
			base.OnDataChanged();
			var backEnd = (Xwt.GtkBackend.CellViewBackend) this.BackendHost.Backend;
			BuildOutputNode = (FilteredBuildOutputNode) backEnd.TreeModel.GetValue (backEnd.CurrentIter, 0);
		}

		Color GetSelectedColor ()
		{
			if (UseStrongSelectionColor) {
				return StrongSelectionColor;
			}
			return SelectionColor;
		}

		void UpdateTextColor (Context ctx)
		{
			if (UseStrongSelectionColor && Selected) {
				ctx.SetColor (StrongSelectionColor);
			} else {
				ctx.SetColor (CellColor);
			}
		}

		bool IsBackgroundColorFieldSet ()
		{
			return GetValue (HasBackgroundColorField, false);
		}

		void FillCellBackground (Context ctx)
		{
			if (Selected) {
				FillCellBackground (ctx, GetSelectedColor ());
			} else if (IsBackgroundColorFieldSet ()) {
				FillCellBackground (ctx, BackgroundColor);
			}
		}

		void FillCellBackground (Context ctx, Color color)
		{
			ctx.Rectangle (BackgroundBounds);
			ctx.SetColor (color);
			ctx.Fill ();
		}
	}
}
