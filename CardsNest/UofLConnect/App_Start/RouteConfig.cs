﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UofLConnect
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Auth", action = "Login" }
            );

            routes.MapRoute(
                name: "ChatRoom",
                url: "chat",
                defaults: new { controller = "Chat", action = "Index" }
            );

            routes.MapRoute(
                name: "GetContactConversations",
                url: "contact/conversations/{contact}",
                defaults: new { controller = "Chat", action = "ConversationWithContact", contact = "" }
            );

            routes.MapRoute(
                name: "PusherAuth",
                url: "pusher/auth",
                defaults: new { controller = "Auth", action = "AuthForChannel" }
            );

            routes.MapRoute(
                name: "SendMessage",
                url: "send_message",
                defaults: new { controller = "Chat", action = "SendMessage" }
            );

            routes.MapRoute(
                name: "MessageDelivered",
                url: "message_delivered/{message_id}",
                defaults: new { controller = "Chat", action = "MessageDelivered", message_id = "" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
