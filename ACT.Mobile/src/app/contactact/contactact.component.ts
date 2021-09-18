import { Component, OnInit } from '@angular/core';
import { PopoverController, NavParams } from '@ionic/angular';

@Component({
  selector: 'app-contactact',
  templateUrl: './contactact.component.html',
  styleUrls: ['./contactact.component.scss'],
})
export class ContactactComponent implements OnInit
{

  constructor( public popCtrl: PopoverController, public nav: NavParams )
  {

  }

  ngOnInit()
  {

  }

  async Call()
  {
    this.popCtrl.dismiss( 1 );
  }

  async Cancel()
  {
    this.popCtrl.dismiss( 0 );
  }

}
