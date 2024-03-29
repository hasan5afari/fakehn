﻿using FakeHN.BLL;
using FakeHN.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace FakeHN.UIL
{
    public partial class Panel : System.Web.UI.Page
    {

        private User user;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Check if user already logged in or not .
                if (!Request.Cookies.AllKeys.Contains("userid"))
                {
                    Response.Redirect("login.aspx");
                }
                else
                {
                    // load user
                    int userid = Convert.ToInt32(Request.Cookies["userid"].Value);
                    UserManager userManager = new UserManager();
                    user = userManager.getUser(userid);

                    panelUserINFO.InnerHtml =
                    $@"<a href='panel.aspx'>{user.name}  {user.family} ({user.username})</a> <span>&nbsp;</span>";

                    Button logoutBtn = new Button();
                    logoutBtn.Text = "[ Logout ]";
                    logoutBtn.ID = $"logoutButton";
                    logoutBtn.Attributes["class"] = "btn bg-transparent";
                    logoutBtn.Click += new EventHandler((s, ee) => LogoutButtonClick(s, ee));

                    if (user.username.Trim() == "admin")
                    {
                        userManagementButton.Visible = true;
                    }

                    panelUserINFO.Controls.Add(logoutBtn);
                }

                // load user posts
                LoadUserPosts();
            }
            catch (BllException ex)
            {
                ExceptionManager exceptionManager = new ExceptionManager();
                exceptionManager.saveException("panel -> Page_Load() -> " + ex.Message_);
            }
        }
        protected void LoadUserPosts()
        {
            try
            {
                PostManager postManager = new PostManager();
                List<Post> userPosts = postManager.getUserPosts(user.userid);

                if (userPosts.Count > 0)
                {
                    for (int i = 0; i < userPosts.Count; i++)
                    {
                        TableRow row = new TableRow();

                        TableCell postNumber = new TableCell();
                        TableCell postBody = new TableCell();
                        TableCell edit = new TableCell();
                        TableCell remove = new TableCell();

                        Button editButton = new Button();
                        Button removeButton = new Button();

                        editButton.Text = "Edit";
                        editButton.ID = $"editPost{userPosts[i].postid}";
                        editButton.Attributes["class"] = "btn btn-primary";
                        editButton.Click += new EventHandler((s, e) => EditPostClick(s, e));
                        editButton.CommandArgument = userPosts[i].postid.ToString();

                        removeButton.Text = "Remove";
                        removeButton.ID = $"removePost{userPosts[i].postid}";
                        removeButton.Attributes["class"] = "btn btn-primary";
                        removeButton.Click += new EventHandler((s, e) => RemovePostClick(s, e));
                        removeButton.CommandArgument = userPosts[i].postid.ToString();

                        postNumber.Text = $"{i + 1}";
                        postBody.Text = userPosts[i].body;
                        edit.Controls.Add(editButton);
                        remove.Controls.Add(removeButton);

                        row.Cells.Add(postNumber);
                        row.Cells.Add(postBody);
                        row.Cells.Add(edit);
                        row.Cells.Add(remove);

                        panelContent.Controls.Add(row);
                    }
                }
                else
                {
                    panelPostsResult.InnerText = "There is no post to show .";
                }
            }
            catch (BllException ex)
            {
                ExceptionManager exceptionManager = new ExceptionManager();
                exceptionManager.saveException("panel -> LoadUserPosts() -> " + ex.Message_);
            }
        }

        protected void EditPostClick(object sender, EventArgs e)
        {
            try
            {
                int postid = Convert.ToInt32(((Button)sender).CommandArgument);
                Response.Redirect("editPost.aspx/?PID=" + Server.UrlEncode(postid.ToString()));

            }
            catch (BllException ex)
            {
                ExceptionManager exceptionManager = new ExceptionManager();
                exceptionManager.saveException("panel -> EditPostClick() -> " + ex.Message_);
            }
        }

        protected void RemovePostClick(object sender, EventArgs e)
        {
            try
            {
                int postid = Convert.ToInt32(((Button)sender).CommandArgument);

                // remove post comments
                CommentManager commentManager = new CommentManager();
                if (commentManager.removePostComments(postid))
                {
                    // remove post itself
                    PostManager postManager = new PostManager();
                    if (postManager.removePost(postid))
                    {
                        Response.Redirect(Request.RawUrl);
                    }
                }
            }
            catch (BllException ex)
            {
                ExceptionManager exceptionManager = new ExceptionManager();
                exceptionManager.saveException("panel -> RemovePostClick() -> " + ex.Message_);
            }
        }

        protected void NewPostButtonClick(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("createPost.aspx");
            }
            catch (BllException ex)
            {
                ExceptionManager exceptionManager = new ExceptionManager();
                exceptionManager.saveException("panel -> NewPostButtonClick() -> " + ex.Message_);
            }
        }

        protected void UserManagementButtonClick(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("userManagementPanel.aspx");
            }
            catch (BllException ex)
            {
                ExceptionManager exceptionManager = new ExceptionManager();
                exceptionManager.saveException("panel -> UserManagementButtonClick() -> " + ex.Message_);
            }
        }

        protected void LogoutButtonClick(object sender, EventArgs e)
        {
            try
            {
                //Check if Cookie exists.
                if (Request.Cookies["userid"] != null)
                {
                    HttpCookie nameCookie = Request.Cookies["userid"];
                    nameCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(nameCookie);

                    Response.Redirect("index.aspx");
                }
            }
            catch (BllException ex)
            {
                ExceptionManager exceptionManager = new ExceptionManager();
                exceptionManager.saveException("panel -> LogoutButtonClick() -> " + ex.Message_);
            }
        }
    }
}