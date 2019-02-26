using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Video.API.Utilities;
using NUnit.Framework;
using VideoApi.Common;
using VideoApi.Contract.Responses;

namespace VideoApi.UnitTests.Utilities
{
    public class PaginationBuilderTest
    {
        private const string SomeResourceUrl = "/api/hearings/participants/checklists";

        [Test]
        public void should_return_page_one_even_if_no_results_exist()
        {
            var response = GetBuilder()
                .ResourceUrl(SomeResourceUrl)
                .WithSourceItems(GetQueryableItems())
                .Build();

            response.CurrentPage.Should().Be(1);
            response.TotalPages.Should().Be(1);
        }

        [Test]
        public void should_return_items_from_given_page()
        {
            var response = GetBuilder()
                .ResourceUrl(SomeResourceUrl)
                .PageSize(2)
                .CurrentPage(2)
                .WithSourceItems(GetQueryableItems("apple", "orange", "pear", "banana"))
                .Build();

            response.CurrentPage.Should().Be(2);
            response.Items.Should().BeEquivalentTo("pear", "banana");
        }

        [Test]
        public void should_echo_given_page_size()
        {
            var response = GetBuilder()
                .ResourceUrl(SomeResourceUrl)
                .PageSize(2)
                .CurrentPage(1)
                .WithSourceItems(GetQueryableItems("apple", "orange"))
                .Build();

            response.PageSize.Should().Be(2);
        }

        [Test]
        public void should_include_page_size_and_number_in_next_and_previous_links()
        {
            var response = GetBuilder()
                .ResourceUrl(SomeResourceUrl)
                .PageSize(1)
                .CurrentPage(2)
                .WithSourceItems(GetQueryableItems("ford", "volvo", "audio"))
                .Build();

            response.NextPageUrl.Should().Contain("page=3").And.Contain("pageSize=1");
            response.PrevPageUrl.Should().Contain("page=1").And.Contain("pageSize=1");
        }

        [Test]
        public void should_not_have_a_next_link_on_last_page()
        {
            var response = GetBuilder()
                .ResourceUrl(SomeResourceUrl)
                .PageSize(1)
                .CurrentPage(2)
                .WithSourceItems(GetQueryableItems("bronze", "iron"))
                .Build();

            response.NextPageUrl.Should().BeNull();
        }

        [Test]
        public void should_not_have_a_prev_link_on_first_page()
        {
            var response = GetBuilder()
                .ResourceUrl(SomeResourceUrl)
                .PageSize(2)
                .WithSourceItems(GetQueryableItems("apple", "orange"))
                .Build();

            response.PrevPageUrl.Should().BeNull();
        }

        [Test]
        public void should_be_missing_both_prev_and_next_pages_if_there_is_only_one_page()
        {
            var response = GetBuilder()
                .ResourceUrl(SomeResourceUrl)
                .PageSize(5)
                .WithSourceItems(GetQueryableItems("item1"))
                .Build();

            response.CurrentPage.Should().Be(1);
            response.TotalPages.Should().Be(1);
        }

        [Test]
        public void should_throw_exception_if_page_is_outside_range()
        {
            When(() =>
                GetBuilder()
                    .WithSourceItems(GetQueryableItems())
                    .ResourceUrl(SomeResourceUrl)
                    .CurrentPage(2)
                    .Build()
            ).Should().Throw<BadRequestException>();
        }

        [Test]
        public void should_fail_on_invalid_page_number()
        {
            When(() => GetBuilder().CurrentPage(0)).Should().Throw<ArgumentException>();
        }

        [Test]
        public void should_fail_on_invalid_page_size()
        {
            When(() => GetBuilder().PageSize(0)).Should().Throw<ArgumentException>();
        }

        [Test]
        public void should_throw_exception_if_resource_url_is_not_set()
        {
            When(() => GetBuilder().Build()).Should().Throw<InvalidOperationException>();
        }

        private PaginationBuilder<StubPagedResponse, string> GetBuilder()
        {
            return new PaginationBuilder<StubPagedResponse, string>(items => new StubPagedResponse {Items = items});
        }

        /// <summary>Help wrapper to build catch clauses</summary>
        private Action When(Action action)
        {
            return action;
        }

        private IQueryable<string> GetQueryableItems(params string[] items)
        {
            return items.AsQueryable();
        }

        public class StubPagedResponse : PagedResponse
        {
            public List<string> Items { get; set; }
        }
    }
}
