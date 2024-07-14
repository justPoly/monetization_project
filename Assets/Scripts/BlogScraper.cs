using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class BlogScraper : MonoBehaviour
{
    [SerializeField] private string blogUrl = "https://www.maliyo.com/blog/";
    [SerializeField] private GameObject blogPostPrefab; // Prefab for displaying blog posts
    [SerializeField] private Transform contentParent; // Parent transform to instantiate blog post prefabs
    private List<BlogPost> blogPosts = new List<BlogPost>(); // List to store blog post data

    void Start()
    {
        StartCoroutine(LoadBlogPosts());
    }

    IEnumerator LoadBlogPosts()
    {
        UnityWebRequest www = UnityWebRequest.Get(blogUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load blog: " + www.error);
            yield break;
        }

        string htmlContent = www.downloadHandler.text;
        Debug.Log("HTML Content Loaded: " + htmlContent.Substring(0, Mathf.Min(htmlContent.Length, 500))); // Log the first 500 characters of the HTML content

        // Parse HTML content to extract blog post titles and images
        ExtractBlogPosts(htmlContent);

        // Display or use blog post data
        DisplayBlogPosts();
    }

    void ExtractBlogPosts(string html)
    {
        blogPosts.Clear();

        // Regular expression to match <article> tags
        string articlePattern = @"<article.*?>(.*?)<\/article>";
        MatchCollection articleMatches = Regex.Matches(html, articlePattern, RegexOptions.Singleline);

        foreach (Match match in articleMatches)
        {
            if (blogPosts.Count >= 5) break;

            string articleHtml = match.Groups[1].Value;

            // Regular expression to match the title
            string titlePattern = @"<h3 class=\""elementor-post__title\"">.*?<a.*?>(.*?)<\/a>";
            Match titleMatch = Regex.Match(articleHtml, titlePattern, RegexOptions.Singleline);
            string titleHtml = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : string.Empty;

            // Decode HTML entities in the title
            string title = DecodeHtmlEntities(titleHtml);

            // Regular expression to match the image URL
            string imagePattern = @"<img.*?src=\""([^\""]*?)\"".*?class=\""attachment-full size-full";
            Match imageMatch = Regex.Match(articleHtml, imagePattern, RegexOptions.Singleline);
            string imageUrl = imageMatch.Success ? imageMatch.Groups[1].Value.Trim() : string.Empty;

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(imageUrl))
            {
                blogPosts.Add(new BlogPost { Title = title, ImageUrl = imageUrl });
                Debug.Log("Extracted Title: " + title);
                Debug.Log("Extracted Image URL: " + imageUrl);
            }
        }

        Debug.Log("Total Blog Posts Extracted: " + blogPosts.Count);
    }

    void DisplayBlogPosts()
    {
        foreach (var post in blogPosts)
        {
            GameObject blogPostGO = Instantiate(blogPostPrefab, contentParent);
            BlogPostUI blogPostUI = blogPostGO.GetComponent<BlogPostUI>();
            blogPostUI.SetPost(post.Title, post.ImageUrl);
            Debug.Log("Instantiated Blog Post Prefab: " + post.Title); // Log the instantiation of the prefab
        }
    }

    // Method to decode HTML entities using System.Net.WebUtility.HtmlDecode
    string DecodeHtmlEntities(string input)
    {
        return System.Net.WebUtility.HtmlDecode(input);
    }
}

[System.Serializable]
public class BlogPost
{
    public string Title;
    public string ImageUrl;
}
