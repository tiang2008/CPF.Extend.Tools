using CPF;
using CPF.Controls;
using System;

namespace CPF.Extend.Tools.Controls.Page
{
    public class PageControl : Control
    {
        // 当前页
        [PropertyMetadata(1)]
        public int CurrentPage
        {
            get
            {
                return (int)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        // 每页条数
        [PropertyMetadata(10)]
        public int PageSize
        {
            get
            {
                return (int)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        // 总条目数
        public int TotalItems
        {
            get
            {
                return (int)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        // 总页数
        [Computed(nameof(TotalItems),nameof(PageSize))]
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);


        public event EventHandler<int> PageChanged
        {
            add
            {
                AddHandler(value);
            }
            remove
            {
                RemoveHandler(value);
            }
        }

        protected void OnPageChanged(int i)
        {
            OnPageChanged(new PageChangedEventArgs(i));
        }

        protected virtual void OnPageChanged(PageChangedEventArgs e)
        {
            RaiseEvent(in e, nameof(PageChanged));
        }

        protected override void InitializeComponent()
        {
            Children.Add(new StackPanel
            {
                Children =
                {
                     new WrapPanel {
                         Background = "rgba(0, 0, 0, 0)", 
                         CornerRadius = "0",
                         Foreground = "rgba(0, 0, 0, 85)",
                         Children = 
                         {
                            new Button {
                                Background = "rgb(255, 255, 255)",
                                CornerRadius = "2,0,0,2", 
                                FontSize = 12, 
                                Foreground = "rgb(210, 210, 210)", 
                                Content =  "上一页",
                                Width = 68, 
                                Height = 30,
                                Commands=
                                {
                                     {
                                         nameof(Button.Click),
                                         nameof(PreviousPage),
                                         this
                                     }
                                }
                            }, 
                            new Button 
                            {
                                Background = "#009688",
                                CornerRadius = "0", 
                                FontSize = 12, 
                                Foreground = "rgb(255, 255, 255)", 
                                Content = "1", 
                                Width = 38.6875, 
                                Height = 30
                            }, 
                            new Button 
                            {
                                Background = "rgb(255, 255, 255)", 
                                CornerRadius = "0", 
                                FontSize = 12, 
                                Foreground = "rgb(51, 51, 51)",
                                Content = "2", 
                                Width = 38.6875, 
                                Height = 30
                            }, 
                            new Button 
                            {
                                Background = "rgb(255, 255, 255)",
                                CornerRadius = "0", 
                                FontSize = 12,
                                Foreground = "rgb(51, 51, 51)",
                                Content = "3",
                                Width = 38.6875,
                                Height = 30
                            }, 
                             new Button {
                                Background = "rgb(255, 255, 255)", 
                                CornerRadius = "0", 
                                FontSize = 12, 
                                Foreground = "rgb(51, 51, 51)",
                                Content = "4",
                                Width = 38.6875,
                                Height = 30
                            }, 
                            new Button 
                            {
                                Background = "rgb(255, 255, 255)",
                                CornerRadius = "0",
                                FontSize = 12, 
                                Foreground = "rgb(51, 51, 51)", 
                                Content ="5",
                                Width = 38.6875, 
                                Height = 30
                            }, 
                            new Label 
                            {
                                Background = "rgb(255, 255, 255)",
                                CornerRadius = "0", 
                                FontSize = 12,
                                Foreground = "rgb(153, 153, 153)", 
                                Text = "…", 
                                Width = 44, 
                                Height = 30
                            }, 
                             new Button {
                                Background = "rgb(255, 255, 255)", 
                                CornerRadius = "0", 
                                FontSize = 12,
                                Foreground = "rgb(51, 51, 51)",
                                Content =  "10",
                                Width = 45.359375,
                                Height = 30
                            }, 
                             new Button 
                             {
                                 Background = "rgb(255, 255, 255)", 
                                 CornerRadius = "0,2,2,0",
                                 FontSize = 12, 
                                 Foreground = "rgb(51, 51, 51)",
                                 Content =  "下一页",
                                 Width = 68,
                                 Height = 30,
                                 Commands=
                                 {
                                     {
                                         nameof(Button.Click),
                                         nameof(NextPage),
                                         this
                                     }
                                 }
                             }
                         }, 
                         Width = 423.2f, 
                         Height = 35f
                    },
                }
            });
        }

        public void GoToPage(int page)
        {
            if (page < 1)
                page = 1;
            if (page > TotalPages)
                page = TotalPages;

            CurrentPage = page;
            OnPageChanged(CurrentPage); // 触发事件
        }

        public void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                OnPageChanged(CurrentPage);
            }
        }

        public void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                OnPageChanged(CurrentPage);
            }
        }
    }

    public class PageChangedEventArgs : EventArgs
    {
        public PageChangedEventArgs(int page)
        {
            this.page = page;
        }

        public int page { get; set; }
    }
}
