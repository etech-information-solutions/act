import { Component, OnInit, ViewChild } from '@angular/core';

import { MenuController, AlertController, IonContent } from '@ionic/angular';
import { UserService } from '../services/user.service';

import * as HighCharts from 'highcharts';

@Component({
  selector: 'app-home',
  templateUrl: './home.page.html',
  styleUrls: ['./home.page.scss'],
})
export class HomePage implements OnInit
{
  @ViewChild( IonContent, { static: true } ) content: IonContent;
  
  constructor( public menuCtrl: MenuController, public alertCtrl: AlertController, public auth: UserService ) 
  {
    if ( this.auth.CurrentUser == undefined || ( this.auth.CurrentUser != undefined && this.auth.CurrentUser.Id <= 0 ) )
    {
      this.auth.GoToPage( "login" );
    }

    if ( this.auth.IsTransporter )
    {
      this.auth.GoToPage( "outstandingreport" );
    }

    this.menuCtrl.enable( true );
  }

  async ngOnInit() 
  {
    this.auth.PushNotificationConfig();

    await this.ListOutstandingPalletsPerClient();
    await this.ListOutstandingPalletsPerSite();
  }

  ngAfterViewInit() 
  {
    this.auth.ContentPage = this.content;
  }

  async ListOutstandingPalletsPerSite()
  {
    await this.auth.ListOutstandingPalletsPerSite();

    var series = [];
    var categories = [];
    
    var d1 = [],
        d2 = [],
        d3 = [],
        d4 = [];

    if ( this.auth.OutstandingPalletsPerSite != undefined && this.auth.OutstandingPalletsPerSite.length > 0 )
    {
      var data = [];
      for ( let c of this.auth.OutstandingPalletsPerSite )
      {
        data.push( c.Total );
        categories.push( c.SiteName );

        d1.push( c.Month1 );
        d2.push( c.Month2 );
        d3.push( c.Month3 );
        d4.push( c.Month4 );
      }

      series.push( { name: "0-3 Months", data: d1 } );
      series.push( { name: "4-6 Months", data: d2 } );
      series.push( { name: "7-12 Months", data: d3 } );
      series.push( { name: "> 12 Months", data: d4 } );
    }

    HighCharts.chart( "outstanding-pallets-per-site", 
    {
      chart: {
        type: 'bar'
      },
      title: {
        text: ''
      },
      xAxis: {
        categories: categories,
        title: {
            text: null
        }
      },
      yAxis: {
        min: 0,
        title: {
          text: ''
        },
        labels: {
            overflow: 'justify'
        }
      },
      plotOptions: {
          bar: {
              dataLabels: {
                  enabled: true
              }
          },
          series: {
            stacking: 'normal'
        }
      },
      series: series
    }, function ()
    {
      
    });
  }

  async ListOutstandingPalletsPerClient()
  {
    await this.auth.ListOutstandingPalletsPerClient();

    var series = [];
    var spline = [];
    var categories = [];

    if ( this.auth.OutstandingPalletsPerClient != undefined && this.auth.OutstandingPalletsPerClient.length > 0 )
    {
      var data = [];
      for ( let c of this.auth.OutstandingPalletsPerClient )
      {
        spline.push( c.Past30 );
        data.push( c.Total );
        categories.push( c.ClientName );
      }

      series.push( { type: 'column', name: "Outstanding", data: data } );

      series.push( { type: 'spline', name: "> 30 Days", data: spline, marker: {
          lineWidth: 2,
          lineColor: "#002e70",
          fillColor: 'white'
      } } );
    }

    HighCharts.chart( "outstanding-pallets-per-client",
    {
      title: {
        text: ''
      },
      xAxis: {
        categories: categories
      },
      yAxis: {
        min: 0,
        title: {
          text: 'Outstanding Pallets'
        },
        labels: {
            overflow: 'justify'
        }
      },
      plotOptions: {
          column: {
              dataLabels: {
                  enabled: true
              }
          }
      },
      series: series
    }, function ()
    {
      
    });
  }

  async Refresh( refresher: any )
  {
    await this.ListOutstandingPalletsPerClient();
    await this.ListOutstandingPalletsPerSite();
    
    refresher.target.complete();
  }

  async ShowDetail( page: any )
  {
    if ( page == 1)
    {
      this.auth.GoToPage( "outstandingpalletsperclient" );
    }
    else
    {
      this.auth.GoToPage( "outstandingpalletspersite" );
    }
  }
}
