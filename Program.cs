﻿using FB.QuickCommenter.Helpers;
using FB.QuickCommenter.Model;
using System;
using System.Threading.Tasks;

namespace FB.QuickCommenter {
  class Program {
    static async Task Main() {
      var fbApiAddress = "https://graph.facebook.com/v15.0/";
      Console.Write("Введите access token:");
      var token = Console.ReadLine();
      var cs = new ConnectSettings() {
        Token = token
      };
      ProxyHelper.FillProxy(cs);
      var re = new RequestExecutor(fbApiAddress, cs);
      var fpm = new FanPageManager(re);
      var fp = await fpm.SelectFanPageAsync();
      var posts = await fpm.GetPostIdsAsync(fp.Id, fp.Token);
      if (posts.Count == 0) {
        Console.WriteLine("Постов нет. Выхожу...");
        System.Environment.Exit(1);
      } else {
        for (var i = 0; i < posts.Count; i++) {
          Console.WriteLine($"{i + 1}. {posts[i].Item2}");
        }
        Console.WriteLine("Q. Прокомментировать все посты");
        Console.Write("Выберите пост:");
        var user_input = Console.ReadLine();
        if (user_input.Equals("Q") || user_input.Equals("q")) {
          for (var index = 0; index < posts.Count; index++) {
            var postId = posts[index].Item1;
            var comments = CommentsHelper.GetComments();
            Console.WriteLine($"Найдено {comments.Count} комментариев!");
            var photos = PhotosHelper.GetPhotos();
            Console.WriteLine($"Найдено {photos.Count} фотографий!");
            await BulkHelper.BulkProcessAsync(fbApiAddress, async (re, cs) => {
              if (comments.Count == 0) {
                Console.WriteLine("Комментарии кончились!");
                return;
              }
              if (photos.Count == 0) {
                Console.WriteLine("Фотографии кончились!");
                return;
              }
              var c = comments[0];
              comments.RemoveAt(0);
              var p = photos[0];
              photos.RemoveAt(0);
              Console.WriteLine($"Оставляем коммент {c} с фотографией {p}");
              var cm = new CommentsManager(re);
              await cm.AddCommentAsync(c, postId, p);
            });
          }
          Console.ReadKey();
        } else {
          var index = int.Parse(user_input) - 1;
          var postId = posts[index].Item1;
          var comments = CommentsHelper.GetComments();
          Console.WriteLine($"Найдено {comments.Count} комментариев!");
          var photos = PhotosHelper.GetPhotos();
          Console.WriteLine($"Найдено {photos.Count} фотографий!");
          await BulkHelper.BulkProcessAsync(fbApiAddress, async (re, cs) => {
            if (comments.Count == 0) {
              Console.WriteLine("Комментарии кончились!");
              return;
            }
            if (photos.Count == 0) {
              Console.WriteLine("Фотографии кончились!");
              return;
            }
            var c = comments[0];
            comments.RemoveAt(0);
            var p = photos[0];
            photos.RemoveAt(0);
            Console.WriteLine($"Оставляем коммент {c} с фотографией {p}");
            var cm = new CommentsManager(re);
            await cm.AddCommentAsync(c, postId, p);
          });
          Console.ReadKey();
        }
      }
    }
  }
}